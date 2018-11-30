# Create the SQL Server database from scratch
# Run this from the "beta-private" directory of the repo

# 0. Remove the existing data
docker volume rm {api_source_data,beta-private_db_data}

# 1. Create volumes to hold the source SQL files and SQL Server data
docker volume create api_source_data
docker volume create beta-private_db_data

# 2. Run SQL server container with the source and data volumes mounted in the correct places 
docker run --rm -d -it \
  --name setup_sql_server \
  -v api_source_data:/opt/data \
  -v beta-private_db_data:/var/opt/mssql \
  -e ACCEPT_EULA=Y -e SA_PASSWORD=SA@Password \
  microsoft/mssql-server-linux:2017-latest

# 3. Copy the source SQL files and data into the running container 
docker cp api/NHSD.GPITF.BuyingCatalog/Data/. setup_sql_server:/opt/data

# 4. Run the from-scratch script in the running SQL Server container 
docker exec -it setup_sql_server /bin/bash -c "cd opt/data; ./FromScratch.MSSQLServer.sh"

# 5. Stop the setup container 
docker stop setup_sql_server
