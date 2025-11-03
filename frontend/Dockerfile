FROM node:20-alpine as build
WORKDIR /app
COPY ./frontend/ ./
RUN npm install
RUN npm run build:prod

FROM nginx:alpine
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

