# Transaction ingestion Console App

## Overview

This Project is .Net  Console App that shows the ingestion of transactions, It processes data from Json file, which is mocking an API and various operations like Upsertion, transaction Update ans status changes to Transaction are performed.
This application also tracks changes to record and stores in Audit table and this application uses .Net along with Entity Framework core with SQLite.

## Features
* Feeding data from a json file
* Transcation Upsertion using Transaction Id
* Track audit changes
* Status change to Finalized when record crosses over 24 hours
* Status change to Revoked for missing Transactions

## Structure
<img width="221" height="308" alt="image" src="https://github.com/user-attachments/assets/759e3b2d-0962-4f7c-9b8c-c6a4929fd9e1" />

## Technologies Used

* .Net 10
* Entity Framework core
* SQLite

## Database
Transcations DB
* Tables : Transactions, Audit

## Design Decisions

### DTO
'Transaction DTO' (data transfer object)is used to seperate exteral data exposure from internal database tables, this allows to store only 4 digits of card along with flexibility if API changes.

### Idempotency
Hashset is used to achieve idempotency, such that no duplicate records are present in database

### Audit Log

A seperate table is presnet to track changes to fields in Transaction table, uses old vs new value along with timestamp.

### Revocation and Finalization

* Any records within the last 24 hours missing form the latest snap shot are marked as revoked
* Transactions older than 24 hours are marked as finalized

## Assumtions

* Transactions Id is unique
* Only last 4 digits of Card number is stored
* Date used in json file may not be applicable to some test cases while evaluating
* Time is in UTC format.

## Set up Instructions:

### 1. clone the repository
```bash
git clone https://github.com/AvulaUdayKumarReddy/Transactions-Ingest.git
cd Transaction-Ingest.git
```
### 2. Install dependencies

* Microsoft.EntityFrameworkCore.Sqlite
* Microsoft.EntityFrameworkCore.Tools
* Microsoft.EntityFrameworkCore.Design
* Microsoft.Extensions.Configuration
* Microsoft.Extensions.Configuration.Json
* Microsoft.Extensions.Hosting

### 3. Apply migrations

```bash
Add-Migration InitialMigration
Update-Database
```
### 4. Run the app
  
  

  






