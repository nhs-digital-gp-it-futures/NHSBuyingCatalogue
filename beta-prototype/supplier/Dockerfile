FROM node:carbon-alpine
EXPOSE 8000
WORKDIR /home/node/app
COPY package.json .
COPY package-lock.json .
COPY lib ./lib
RUN npm install
COPY . .

CMD ["npm", "start"]
