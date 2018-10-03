FROM node:8 AS base
WORKDIR /home/node/app

FROM base AS dependencies
COPY package*.json ./
COPY lib ./lib
RUN npm install

FROM dependencies AS pre-build
COPY . .

FROM pre-build AS test
#COPY __mocks__ __mocks__/
#COPY __tests__ __tests__/
#COPY jest.setup.js .
RUN npm test

FROM pre-build AS build
RUN npm run build

FROM node:8-alpine AS release
COPY --from=dependencies /home/node/app/package*.json ./
RUN npm install --production
COPY --from=build /home/node/app/server.js .
COPY --from=build /home/node/app/static static/
CMD ["npm", "start"]
