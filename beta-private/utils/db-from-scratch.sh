# Create the SQL Server database from scratch
# Run this from the "beta-private" directory of the repo

# 1. Create a volume to hold the source SQL files and data.
 docker volume create api_source_data

# 2. Create a volume to hold the SQL Server data for the private beta 
docker volume create beta-private_db_data

# 3. Run SQL server container with the source and data volumes mounted in the correct places 
docker run --rm -d -it \
  --name setup_sql_server \
  -v api_source_data:/opt/data \
  -v beta-private_db_data:/var/opt/mssql \
  -e ACCEPT_EULA=Y -e SA_PASSWORD=SA@Password \
  microsoft/mssql-server-linux:2017-latest

# 4. Copy the source SQL files and data into the running container 
docker cp api/NHSD.GPITF.BuyingCatalog/Data/. setup_sql_server:/opt/data

# 5. Run the from-scratch script in the running SQL Server container 
docker exec -it setup_sql_server /bin/bash -c "cd opt/data; ./FromScratch.MSSQLServer.sh"

# 6. Stop the setup container 
docker stop setup_sql_server
