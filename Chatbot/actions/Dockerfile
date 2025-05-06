FROM rasa/rasa-sdk:latest

USER root

# Install the dateparser library
RUN pip install dateparser
RUN pip install pytz

# Copy the actions folder into the container
COPY ./ /app/actions