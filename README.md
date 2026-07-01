# StreakHub

## Giới thiệu

StreakHub là hệ thống quản lý công việc kết hợp theo dõi chuỗi hoàn thành (Daily Streak), giúp người dùng xây dựng và duy trì thói quen học tập, làm việc, chia sẻ và trao đổi lịch trình, tìm kiếm những lịch trình được nhiêu người yêu thích.

## Công nghệ sử dụng

- ASP.NET Core 10
- Entity Framework Core
- PostgreSQL (Neon)
- Docker
- Docker Compose
- Git & GitHub

## Chức năng

- Đăng ký, đăng nhập
- Quản lý Todo
- Theo dõi Daily Streak
- Lịch (Calendar)
- Nhắc nhở qua Email
- Bảng xếp hạng (Ranking)

## Yêu cầu

- Docker Desktop
- Docker Compose

## Chạy dự án bằng Docker

Clone project:

```bash
git clone <repository-url>
cd StreakHub.API
```

Tạo file `.env`:

```env
DB_CONNECTION=your_connection_string
SMTP_PASSWORD=your_gmail_app_password
```

Khởi động:

```bash
docker compose up --build
```

Dừng:

```bash
docker compose down
```

## Cấu trúc dự án

```
StreakHub.API
├── Controllers
├── Services
├── Models
├── DTOs
├── Interfaces
├── Data
├── wwwroot
├── Dockerfile
└── docker-compose.yml
```

## License

Dự án được phát triển phục vụ học tập tại Trường Đại học Công nghệ Giao thông Vận tải.