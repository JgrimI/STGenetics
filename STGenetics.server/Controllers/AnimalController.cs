using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using STGenetics.Shared;
using System.Text;
using System.Text.Json;

namespace STGenetics.Server.Controllers
{
    [ApiController]
    [Route("api/Animal")]
    public class AnimalController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly AppDbContext context;
        private readonly HttpClient _http;

        public AnimalController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration, AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            this.context = context;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _http = httpClientFactory.CreateClient();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [HttpGet("GetAll")]
        public async Task<List<Animal>> Get()
        {
            return context.Animal.ToList();
        }

        [HttpPost("CreateAnimal")]
        public async Task<IActionResult> CreateAnimal([FromBody] Animal animal)
        {
            if (animal == null)
            {
                return BadRequest("Animal data is null");
            }

            // Add your logic to add the new animal to the database
            context.Animal.Add(animal);
            await context.SaveChangesAsync();

            // You might want to return the created animal, including its generated ID
            return CreatedAtAction(nameof(GetAnimalById), new { id = animal.AnimalId }, animal);
        }


        [HttpPut("UpdateAnimal")]
        public async Task<IActionResult> UpdateAnimal([FromBody] Animal animal)
        {
            if (animal == null || animal.AnimalId == 0)
            {
                return BadRequest("Invalid animal data");
            }

            var existingAnimal = await context.Animal.FindAsync(animal.AnimalId);
            if (existingAnimal == null)
            {
                return NotFound($"Animal with ID {animal.AnimalId} not found");
            }

            // Update the properties
            existingAnimal.Name = animal.Name;
            existingAnimal.Breed = animal.Breed;
            existingAnimal.BirthDate = animal.BirthDate;
            existingAnimal.Sex = animal.Sex;
            existingAnimal.Price = animal.Price;
            existingAnimal.Status = animal.Status;

            // Save changes
            await context.SaveChangesAsync();

            //return Ok(existingAnimal);
            return Ok();  
        }

        [HttpDelete("DeleteAnimal/{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            // Find the animal in the database
            var animal = await context.Animal.FindAsync(id);
            if (animal == null)
            {
                // If the animal does not exist, return a NotFound response
                return NotFound($"Animal with ID {id} not found.");
            }

            // Remove the animal from the database
            context.Animal.Remove(animal);

            // Save the changes
            await context.SaveChangesAsync();

            // Return a success response
            return Ok($"Animal with ID {id} has been deleted.");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimalById(int id)
        {
            var animal = await context.Animal.FindAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            return animal;
        }
         
        [HttpGet("GetTotalAnimals")]
        public async Task<ActionResult<int>> GetTotalAnimals()
        {
            int count = await context.Animal.CountAsync();
            return count;
        }

        [HttpGet("GetAnimalPaged")]
        public async Task<ActionResult<IEnumerable<Animal>>> AnimalGet(int page = 1, int pageSize = 10, string? sortBy = null)
        {
            // Calculate the number of records to skip
            int skip = (page - 1) * pageSize;

            // Set default sorting if sortBy is null or empty
            string defaultSortBy = "AnimalId"; // Or any default column you want to sort by
            string sortString = string.IsNullOrEmpty(sortBy) ? defaultSortBy : sortBy;
            // Build your SQL query with pagination
            string sql = "EXECUTE Animal_Get_Paged @Skip, @Take, @SortBy;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("Skip", skip),
                new SqlParameter("Take", pageSize)
            };
 
            parameters.Add(new SqlParameter("SortBy", sortString)); 
        
            // Execute the query with pagination and sorting
            var animals = await context.Animal.FromSqlRaw(sql, parameters.ToArray()).ToListAsync();

            return animals;
        }

        [HttpGet("GetAnimalBreeds")]
        public async Task<ActionResult<IEnumerable<string>>> GetAnimalBreeds()
        {
            // Using Entity Framework to select distinct breeds
            var breeds = await context.Animal
                                      .Select(a => a.Breed)
                                      .Distinct()
                                      .ToListAsync();

            return breeds;
        }

        [HttpPost("SearchAnimals")]
        public async Task<ActionResult<IEnumerable<Animal>>> SearchAnimals([FromBody] JsonElement body)
        {
            var args = GetBodyArgs(body);

            // Extract individual parameters from args
            var name = args["name"] as string ?? string.Empty;
            DateTime? birthDate = null;
            if (DateTime.TryParse(args["birthDate"] as string, out DateTime parsedDate))
            {
                birthDate = parsedDate;
            }
            var sex = args["sex"] as string ?? string.Empty;
            var price = args["price"] as decimal? ?? null;
            var status = args["status"] as string ?? string.Empty;
            var selectedBreeds = args["selectedBreeds"] as string[] ?? Array.Empty<string>();
            var page = 1;
            var pageSize =10;
            var query = context.Animal.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(a => a.Name.Contains(name));
            }
            if (birthDate.HasValue)
            {
                query = query.Where(a => a.BirthDate.Date == birthDate.Value.Date);
            }
            if (!string.IsNullOrWhiteSpace(sex))
            {
                query = query.Where(a => a.Sex == sex);
            }
            if (price.HasValue)
            {
                query = query.Where(a => a.Price == price.Value);
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status == status);
            }
            if (selectedBreeds != null && selectedBreeds.Any())
            {
                query = query.Where(a => selectedBreeds.Contains(a.Breed));
            }

            int skip = (page - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            var result = await query.ToListAsync();
            return  result;
        }

        public Dictionary<string, object?> GetBodyArgs([FromBody] JsonElement body)
        {
            var args = body.EnumerateObject().ToDictionary(
                kvp => kvp.Name,
                kvp =>
                {
                    object value;
                    switch (kvp.Value.ValueKind)
                    {
                        case JsonValueKind.Array:
                            var items = kvp.Value.EnumerateArray().ToArray();
                            var list = new List<object>();

                            foreach (var item in items)
                            {
                                if (item.ValueKind == JsonValueKind.Array)
                                {
                                    list.Add(item.EnumerateArray().Select(x => x.GetString()).ToArray());
                                }
                                else if (item.ValueKind == JsonValueKind.String)
                                {
                                    list.Add(item.GetString());
                                }
                            }
                            value = list.ToArray();
                            break;
                        case JsonValueKind.String:
                            value = kvp.Value.GetString();
                            break;
                        default:
                            throw new InvalidOperationException($"Unexpected JSON type: {kvp.Value.ValueKind}");
                    }
                    return value;
                }
            );

            return args;

        }


    }
}
