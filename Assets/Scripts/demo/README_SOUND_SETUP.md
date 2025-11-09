# Hướng Dẫn Gán Âm Thanh Cho Game

## 1. Thiết Lập SoundManager

### Bước 1: Tạo GameObject SoundManager
1. Trong Scene (Game scene), tạo Empty GameObject:
   - Click chuột phải trong Hierarchy → `Create Empty`
   - Đặt tên: `SoundManager`
2. Thêm Component SoundManager:
   - Chọn GameObject `SoundManager`
   - Click `Add Component` → tìm và chọn `SoundManager`

### Bước 2: Gán AudioClip vào SoundManager
1. Import AudioClip vào Unity:
   - Kéo thả file âm thanh vào thư mục `Assets/Audio` (tạo thư mục nếu chưa có)
   - Đảm bảo file âm thanh có format: `.wav`, `.mp3`, `.ogg`, `.aiff`
2. Trong Inspector của SoundManager, gán các AudioClip:

#### Player Sounds:
- **Player Footstep Sound**: Âm thanh bước chân của player (nên là file ngắn, có thể loop)
- **Player Jump Sound**: Âm thanh nhảy
- **Gem Collect Sound**: Âm thanh thu thập gem
- **Water Splash Sound**: Âm thanh rơi xuống nước

#### Enemy Sounds:
- **Enemy Footstep Sound 1**: Âm thanh cho Enemy Horizontal (di chuyển ngang)
- **Enemy Footstep Sound 2**: Âm thanh cho Enemy Vertical (di chuyển dọc)
- **Enemy Footstep Sound 3**: Âm thanh cho Enemy Frog (nhảy)

#### Ambient Sounds:
- **Water Ambient Sound**: Âm thanh nước chảy (nên là file loop)

### Bước 3: Điều Chỉnh Volume (Tùy chọn)
- **SFX Volume**: Volume cho hiệu ứng âm thanh (0-1)
- **Music Volume**: Volume cho nhạc nền (0-1)

---

## 2. Thiết Lập WaterSoundZone (Cho Âm Thanh Nước Chảy)

### Bước 1: Tạo WaterSoundZone cho mỗi khu vực nước
1. Chọn GameObject nước trong scene (hoặc tạo Empty GameObject)
2. Thêm Collider2D làm Trigger:
   - Add Component → `BoxCollider2D` hoặc `CircleCollider2D`
   - **BẬT** `Is Trigger` = true
   - Điều chỉnh kích thước để bao phủ vùng muốn phát âm thanh
3. Thêm Script:
   - Add Component → `WaterSoundZone`

### Bước 2: Tùy Chỉnh WaterSoundZone
- **Sound Volume**: Volume tối đa (0-1, mặc định 0.4)
- **Fade In Speed**: Tốc độ tăng volume khi vào (mặc định 2)
- **Fade Out Speed**: Tốc độ giảm volume khi ra (mặc định 2)
- **Use Distance Based Volume**: Bật để volume giảm dần theo khoảng cách
- **Max Distance**: Khoảng cách tối đa để nghe (mặc định 10)
- **Min Distance**: Khoảng cách đạt volume tối đa (mặc định 2)

**Lưu ý**: AudioClip sẽ tự động lấy từ SoundManager, không cần gán lại!

---

## 3. Kiểm Tra Setup

### Kiểm tra Player:
- Player phải có tag "Player" (Edit → Project Settings → Tags and Layers)
- Script `PlayerController` và `PlayerCollision` đã được gắn vào Player prefab

### Kiểm tra Enemies:
- Mỗi enemy có script tương ứng (EnemyHorizontal, EnemyVerticalMovement, FrogMovement)
- Script tự động sử dụng âm thanh từ SoundManager

### Kiểm tra Nước:
- Các GameObject nước có tag "Water" hoặc layer "Water" (cho va chạm rơi xuống nước)
- Các khu vực nước có WaterSoundZone (cho âm thanh ambient)

---

## 4. Cấu Trúc Thư Mục Đề Xuất

```
Assets/
├── Audio/
│   ├── Player/
│   │   ├── footstep.wav
│   │   ├── jump.wav
│   │   └── gem_collect.wav
│   ├── Enemy/
│   │   ├── enemy_horizontal.wav
│   │   ├── enemy_vertical.wav
│   │   └── enemy_frog.wav
│   ├── Environment/
│   │   ├── water_splash.wav
│   │   └── water_ambient_loop.wav
│   └── Music/
│       └── background_music.mp3
└── Scripts/
    └── demo/
        ├── SoundManager.cs
        ├── WaterSoundZone.cs
        └── ...
```

---

## 5. Troubleshooting

### Âm thanh không phát:
1. Kiểm tra SoundManager có tồn tại trong scene không
2. Kiểm tra AudioClip đã được gán vào SoundManager
3. Kiểm tra Volume không bị tắt (SFX Volume > 0)
4. Kiểm tra AudioSource trong SoundManager có được tạo tự động không

### Âm thanh nước không phát:
1. Kiểm tra WaterSoundZone có Collider2D với Is Trigger = true
2. Kiểm tra Player có tag "Player"
3. Kiểm tra Water Ambient Sound đã được gán trong SoundManager

### Âm thanh quá to/nhỏ:
- Điều chỉnh Volume trong SoundManager
- Điều chỉnh volume multiplier trong từng script (nếu cần)
- Điều chỉnh volume của AudioClip trong Import Settings

---

## 6. Tối Ưu Hóa

### Import Settings cho AudioClip:
- **Load Type**: Compression (giảm dung lượng)
- **Compression Format**: Vorbis (cho .ogg) hoặc PCM (cho .wav chất lượng cao)
- **Quality**: 70-90% (cân bằng chất lượng và dung lượng)
- **Sample Rate**: 22050 Hz hoặc 44100 Hz (tùy nhu cầu)

### Cho Footstep Sounds:
- Nên dùng file ngắn (0.1-0.3 giây)
- Có thể loop nếu cần
- Compression Format: Vorbis, Quality: 70%

### Cho Ambient Sounds:
- Nên dùng file loop (không có khoảng im lặng ở đầu/cuối)
- Compression Format: Vorbis, Quality: 50-70%

---

Chúc bạn setup thành công! 🎵


