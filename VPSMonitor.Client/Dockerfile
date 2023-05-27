# FROM node:latest as node
# WORKDIR /app
# COPY . .
# RUN npm install
# RUN npm ng build

# EXPOSE 8080
# CMD [ "ng", "serve"]

FROM --platform=$BUILDPLATFORM node:latest as builder

RUN mkdir /project
WORKDIR /project

RUN npm install -g @angular/cli@13

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
CMD ["ng", "serve", "--host", "0.0.0.0"]