# Use an official Python base image
FROM python:3.9-slim

# Set the working directory
WORKDIR /app

# Copy the local code to the container
COPY . .

# Install Python dependencies
RUN pip install --no-cache-dir -r requirements.txt

EXPOSE 5005

# Set the default command to run your app
CMD ["python", "main.py"]