# Junior Feedback Uygulaması

Bu proje, kullanıcıların bir form aracılığıyla feedback gönderebildiği bir web uygulamasıdır. Feedbackler frontend'den API'ye gönderilir, RabbitMQ kuyruğu aracılığıyla alınır ve MongoDB'ye kaydedilir.

## Kullanılan Teknolojiler

### 🖥 Frontend (React)
- React.js
- Axios (API istekleri için)
- Form validasyon

### Backend (.NET)
- ASP.NET Core Web API
- FluentValidation

### Veri Tabanı & Kuyruk
- **MongoDB** (lokalde çalışır)
- **RabbitMQ** (Docker ile ayağa kaldırılır)

---

##  Kurulum Adımları

### 1. Gereksinimler

Aşağıdaki araçların sisteminizde kurulu olması gerekir:

- [Node.js ve npm](https://nodejs.org/)
- [.NET 8 SDK ](https://dotnet.microsoft.com/en-us/download)
- [MongoDB Community Server](https://www.mongodb.com/try/download/community)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

### 2. RabbitMQ’yu Docker Üzerinden Çalıştırma

Aşağıdaki komutu terminale yazalım.
docker run -d --hostname rabbitmq-local --name rabbitmq \
    -p 5672:5672 -p 15672:15672 \
    rabbitmq:3-management

RabbitMQ Management Paneli: http://localhost:15672
- Kullanıcı adı: guest
- Şifre: guest

### 3. MongoDB'yi Lokal Olarak Başlatma

MongoDB'yi yüklediyseniz, servis olarak çalıştığından emin olun:

Varsayılan bağlantı: mongodb://localhost:27017


### 4. Backend (.NET API) Kurulumu

cd backend
dotnet restore
dotnet build
dotnet run

Api isteğini swagger kullanrak yaptığımız adres: https://localhost:7077/swagger/index.html 

### 5. Frontend (React) Kurulumu

cd frontend
npm install
npm start

React uygulaması http://localhost:5173 adresinde çalışır.

### 6. Feedback Verisi Gönderme Süreci

- Kullanıcı, frontend'deki formu doldurur.
- Form verisi API'ye gönderilir (POST /api/feedback)
- API, veriyi RabbitMQ kuyruğuna gönderir.
- ConsumerService, kuyruktan veriyi alır.
- Alınan veri MongoDB'ye kaydedilir.


### Örnek Ortam Ayarları (appsettings.json)

"MongoDbSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "FeedbackTaskDb",
  "CollectionName": "feedbacks"
}


### API Endpoint
POST /api/feedback – Yeni geri bildirim gönder

{
  "name": "Ali",
  "email": "ali@example.com",
  "message": "Çok güzel bir uygulama!"
}
