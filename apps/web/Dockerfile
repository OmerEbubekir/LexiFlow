# ============================================================
# Stage 1: Build (Node.js ile Vite production bundle)
# ============================================================
FROM node:20-alpine AS build
WORKDIR /app

# Bağımlılık dosyalarını önce kopyala — cache optimizasyonu
COPY package*.json ./

# Production + dev bağımlılıklarını yükle (build için devDependencies gerekli)
RUN npm ci

# Kaynak kodun tamamını kopyala
COPY . .

# Build zamanı ortam değişkenleri (docker-compose'dan ARG olarak geçilir)
ARG VITE_API_BASE_URL
ARG VITE_FIREBASE_API_KEY
ARG VITE_FIREBASE_AUTH_DOMAIN
ARG VITE_FIREBASE_PROJECT_ID
ARG VITE_FIREBASE_STORAGE_BUCKET
ARG VITE_FIREBASE_MESSAGING_SENDER_ID
ARG VITE_FIREBASE_APP_ID

# Production build
RUN npm run build

# ============================================================
# Stage 2: Serve (Nginx ile statik dosyaları sun)
# ============================================================
FROM nginx:1.27-alpine AS runtime

# Özel Nginx konfigürasyonu (SPA routing için)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Vite build çıktısını Nginx'in web root'una kopyala
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 \
  CMD wget -qO- http://localhost/health || exit 1

CMD ["nginx", "-g", "daemon off;"]
