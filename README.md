# App

## Animal Management Web Application

### Description
This web application is designed to manage a database of animals, following specific business rules. It offers functionalities for adding, updating, deleting, and filtering animal data, along with a responsive design for compatibility across various devices.

### Features
#### Data
- **Animal Database**: A fictitious database containing 20 animals with the following fields:
  - `AnimalId`
  - `Name`
  - `Breed`
  - `BirthDate`
  - `Sex` (Male/Female)
  - `Price`
  - `Status` (Active/Inactive)

#### Web Application 
- **Animal Page**:
  - **Form**: A form to filter animal data based on various fields. The filtered data is displayed in the grid below.
  - **Grid**:
    - Displays all data from the Animal database.
    - Supports Insert, Update, and Delete operations.
    - Features pagination, displaying 10 animals per page.
    - Includes a footer showing the sum of the `Price` column.
    - Contains a checkbox column for selecting animals, which are then added to the table described below.
    - Dropdown menu in grid lines for selecting items from a list.
  - **Page**:
    - Shows all selected animals from the grid, grouped by breed.
    - Displays the total purchase amount, discount percentage, and shipping amount.

#### Business Rules
- 5% discount on individual animals if more than 5 of the same animal are added.
- Additional 3% discount on the total purchase if more than 10 animals are bought.
- Free shipping for orders with more than 20 animals; otherwise, a shipping charge of $1,000.00 applies.
- No duplicate animals allowed in an order; error messages are displayed for duplicates.

#### Menu
- Navigation between the Default and Animal pages.

#### General
- Responsive design for mobile and tablet compatibility.
- MVC Blazor architecture using .Net 6.
- Sorting options available for each column in the grid.
- Excel-like/batch edit feature for editing multiple grid lines and saving all changes at once.
- Use of Blazorise and Radzen nuget packages.
