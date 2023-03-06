## Description
Hot desk booking system is a system which should designed to automate the reservation of desk in
office through an easy online booking system.

## Features
Administration:
- Manage locations (add/remove, can&#39;t remove if desk exists in location)
- Manage desk in locations (add/remove if no reservation/make unavailable) 

Employees
- Determine which desks are available to book or unavailable.
- Filter desks based on location
- Book a desk for the day.
- Allow reserving a desk for multiple days but now more than a week.
- Allow to change desk, but not later than the 24h before reservation.
- Administrators can see who reserves a desk in location, where Employees can see only that specific
desk is unavailable.

In nugget packet manager console run command Update-database, to apply migrations.

## Structure:
![alt text](https://user-images.githubusercontent.com/109426665/209875936-c37b40b8-f906-4c53-b23a-a1e42a3d64c7.png)
## Stack
- .Net6
- Ms Sql Server
- EntityFrameworkCore
- AutoMapper
- JwtBearer
- XUnit
- Moq


