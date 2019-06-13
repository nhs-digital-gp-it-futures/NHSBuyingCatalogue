# Getting Dev Environment Set Up Correctly

The following steps should enable you to get a dev environment set up for the Buying Catalogue.

## PREREQUISITES
1. Install Docker for Windows (if using a windows device)
2. Install Node.js (https://www.nodejs.org) version 10+
3. Clone the Buying Catalogue Repo

## STEPS TO SUCCEED
1. Add `docker-compose.override.yml` file to the base directory. Content is with another Dev
2. Add `.env` file to base directory. Content is with another Dev
3. Run `docker-compose up` from the base directory (_note - this will take a while_)
4. In a console window (ideally either Powershell or a bash based console), navigate to the `/api/NHSD.GPITF.BuyingCatalog` folder
5. Copy data folder to sql server container using the command below
    - `docker cp ./Data beta-private_db_1:/tmp`
6. ssh onto sql server container using the command below
    - `docker exec -it beta-private_db_1 /bin/bash`
7. in the ssh session, navigate to /tmp/Data
8. Execute the below script to setup the db (see troubleshooting 1 below if this doesn't work)
    `sh FromScratch.MSSQLServer.sh`
9. Verify the install has worked - navigate to `http://localhost:5100/swagger` and try one of the queries listed. if you need credentials for the query, ask a Dev for those

## Troubleshooting

### 1. Unable to add data to dev database
If this occurs, copy the contents from the `/api/NHSD.GPITF.BuyingCatalog/Data/FromScratch.MSSQLServer.sh` file and paste directly into the SSH shell you should have open at this stage. This will bypass the `Not Found` errors that can arise at this stage

## Common Issues
- Sometimes, you may need to run `npm install` in the `frontend` folder for the dockerfile to find all the requisite packages