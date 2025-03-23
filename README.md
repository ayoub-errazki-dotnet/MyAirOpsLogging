
# Microservices

We have two simple microservices: the **Logging Microservice**, which remains on standby to receive incoming logs continuously, and another microservice named **AlphaService** (or **MainService**).

![Project Screenshot](./projectImages/main.png)

The user makes an HTTP request to the **Main Service**, which simulates some processing and then logs the result based on the GET parameter `simulate` (`success`, `info`, `warning`, `fatal`). The log is then published based on its level, and the **Logging Microservice** intercepts the message and eventually logs it into a file.

## Detailed Process

### Example 1: Success Simulation

1. **User Request:**
   ```bash
   GET [MainService]/api/main/process?simulate=success
   ```
   
2. **Response:**
   ```json
   {
     "message": "Microservice (Main) processed the request successfully",
     "id": "8ef349e1-a415-4b8e-addd-6b83d2c322dd"
   }
   ```

### Example 2: Warning Simulation

1. **User Request:**
   ```bash
   GET [MainService]/api/main/process?simulate=warning
   ```
   
2. **Response:**
   ```json
   {
     "message": "Microservice (Main) raised a warning",
     "id": "8ef349e1-a415-4b8e-addd-6b83d2c322dd"
   }
   ```

![Project Screenshot](./projectImages/http_req_success.png)

## Starting RabbitMQ Server

Using Docker, it's simple to start the RabbitMQ server:

1. Pull and run the RabbitMQ container using the command:
   ```bash
   docker run -it --rm --name myrabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   ```

That would be it for the server.

![Project Screenshot](./projectImages/runrabbitMQ.png)

## Building and Running the Microservices

### Logging Microservice

Build and run the Logging Microservice:

![Project Screenshot](./projectImages/build_run_loggingMS.png)

### Main Microservice

Build and run the Main Microservice:

![Project Screenshot](./projectImages/build_run_MainMS.png)

When the Main Microservice starts, it sends a log indicating that the server is up.

After running a few tests at various levels, the logs are received directly.

![Project Screenshot](./projectImages/logging_returned.png)
