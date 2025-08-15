# Launch Instractions

## Docker Setup

1. **Install docker**

    [Instructions here](https://www.docker.com/get-started)

2. **Clone repository**
    
    ```cmd
    git clone https://github.com/AGR-ALS/FitnessTrackerApi.git
    cd .\FitnessTrackerApi\FitnessTracker.Api\
    ```

3. **Build and run**

    ```cmd
    docker compose up
    ```

4. **Acess SwaggerUI**

    Open [localhost:8080](http://localhost:8080/swagger/index.html)

## Local Setup

1. **Install dotnet**

    [Instructions here](https://dotnet.microsoft.com/en-us/download)

2. **Clone repository**
    
    ```cmd
    git clone https://github.com/AGR-ALS/FitnessTrackerApi.git
    cd .\FitnessTrackerApi\FitnessTracker.Api\
    ```

3. **Build and run**

    ```cmd
    dotnet run --launch-profile https
    ```

4. **Access SwaggerUI**

    Open [localhost:7205](https://localhost:7205/swagger/index.html)

