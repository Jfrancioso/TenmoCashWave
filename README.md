# TEnmoCashWave Online Payment System

This is a completed version of the TEnmo online payment system. The product is an online payment service for transferring "TE bucks" between friends. This project includes a RESTful API server and a command-line application.

## Getting Started

### Prerequisites

- SQL Server Management Studio
- Visual Studio
- Postman

### Installation

1. Clone this repository onto your local machine using the following command:

``` bash
git clone https://github.com/Jfrancioso/TenmoCashWave.git
```

2. Open the `tenmo.sql` file located in the `database` folder with SQL Server Management Studio and execute it to set up the database.

3. Open the solution file in Visual Studio.

4. In Visual Studio, right-click the solution and select **Set Startup Projects...**. In the window that appears, select **Multiple startup projects** and set both "TenmoClient" and "TenmoServer" to have the action `Start`.

5. Run the program by pressing F5 or clicking on the "Start" button in the toolbar.

## Usage

Upon running the program, you will be presented with various options to perform different actions such as:

1. Register a new user
2. Log in with your registered username and password
3. View your account balance
4. Send TE bucks to other registered users
5. View your transfers
6. Retrieve transfer details based on transfer ID

Optional features include:

7. Request a transfer of a specific amount of TE bucks from another registered user
8. View your pending transfers
9. Approve or reject a pending transfer

## Built With

- C#
- .NET Framework
- SQL Server

## Authors

Joseph Francioso
John Lindsey
Christaline Ivey
Seth Barnett

## Acknowledgments

- A big thank you to the fantastic students at Tech Elevator that worked on this project!
