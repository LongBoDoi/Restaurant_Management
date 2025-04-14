# 1️⃣ Build stage: build your app inside a Node container
FROM node:20 AS build-stage

WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Copy all app files
COPY . .

# Build the app for production
RUN npm run build

# 2️⃣ Production stage: serve the built app using nginx
FROM nginx:stable-alpine AS production-stage

# Remove default nginx static files
RUN rm -rf /usr/share/nginx/html/*

# Copy built files from build stage to nginx's public folder
COPY --from=build-stage /app/dist /usr/share/nginx/html

# Expose nginx default port
EXPOSE 80

# Start nginx when container starts
CMD ["nginx", "-g", "daemon off;"]
