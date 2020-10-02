# console_LeaveTracker
c# Console Application to search, create, update the leaves entries in csv file.

#For testing:-
Employee ID: 104 (not manager)
Employee ID: 102 (Manager can only update the leaves assigned to him)

# Problem Statement

Develop a Command-Line ***Leave tracker*** application.
Create an console application for creating, updating, listing, searching leaves.

Employee data be read/parsed from a CSV file.  
Refer file `employees.csv` for the sample employee data.

The program should store the leaves in a separate CSV file - `leaves.csv`
Leave data should be stored in Leaves.csv in below format.

## Leave CSV format

| ID | Creator       | Manager     | Title                    | Description           | Start-Date  | End-Date   | Status      |
|----|---------------|-------------|--------------------------|-----------------------|-------------|------------|-------------|
| 1  | Marsha Mellow | Hazel Nutt  |  5 days PTO in June      | Holiday in Hawaii     | 22-06-2020  | 26-06-2020 | Pending     |
| 1  | Olive Yew     | Ann Chovey  |  1 day PTO Thursday      | School admission      | 24-06-2020  | 24-06-2020 | Approved    |

- Status' values = Pending / Approved / Rejected
- Creator Name and Manager name be picked up from the employee id entered by
  user.

## Execution-flow
On executing the console application, prompt user to enter an employee-id to
use. (No authentication needed, just validation of the id is enough)

Prompt user with below choices
1. Create Leave
	- Assign-To (only current user's manager should be allowed)
	- Title
	- Description
	- Start-Date
	- End-Date
	
2. List my Leaves

3. Update leaves  
   This should be accessible only to managers.
   Manager should be able to change the status of leaves assigned to themself
	
3. Search Leave by
	- Title 
	- Status (Pending/Approved/Rejected)


# Considerations
- Validate all the inputs.
- Use proper Exception-handling
- Write Unit-Tests

