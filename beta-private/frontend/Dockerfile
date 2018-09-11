FROM node:8 AS base
WORKDIR /home/node/app

FROM base AS dependencies
COPY package*.json ./
RUN npm install

FROM dependencies AS build
COPY pages pages/
COPY components components/
COPY static static/
COPY styles styles/
COPY next.config.js .
RUN npm run build

FROM node:8-alpine AS release
COPY --from=dependencies /home/node/app/package*.json ./
RUN npm install --production
COPY --from=build /home/node/app/.next .next/
COPY --from=build /home/node/app/static static/
CMD ["npm", "start"]
