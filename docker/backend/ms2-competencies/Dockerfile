# Use a base Python image
FROM python:3.13

# Set the working directory in the container
WORKDIR /app

# Copy the requirements file to the container
COPY backend/competencies/requirements.txt /app/requirements.txt

# Install the dependencies
RUN pip install --no-cache-dir -r /app/requirements.txt

# Copy the rest of the application code to the container
COPY backend/competencies/ /app

# Collect static files
RUN python manage.py collectstatic --noinput

# Expose the port the application will run on
EXPOSE 8003

# Define the default command to run the application
CMD ["gunicorn", "--bind", "0.0.0.0:8003", "competencies.wsgi:application"]