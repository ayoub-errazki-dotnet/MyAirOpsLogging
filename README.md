# Microservices

We have two simple microservices: the **Logging Microservice**, which remains on standby to receive incoming logs continuously, and another microservice named **AlphaService** (or **MainService**).

The user makes an HTTP request to the **Main Service**, which simulates some processing and then logs the result based on the GET parameter `simulate` (`success`, `info`, `warning`, `fatal`). The log is then published based on its level, and the **Logging Microservice** intercepts the message and eventually logs it into a file.

## Detailed Process

### Example 1: Success Simulation

1. **User Request:**
   ```
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
   ```
   GET [MainService]/api/main/process?simulate=warning
   ```
2. **Response:**
   ```json
   {
     "message": "Microservice (Main) raised a warning",
     "id": "8ef349e1-a415-4b8e-addd-6b83d2c322dd"
   }
   ```
