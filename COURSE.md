# คอร์ส C# + Unity ผ่านเกมสนุกเกอร์ (snoker164_1302)

โปรเจกต์เรียนหลัก — เรียนแบบทำไปเรียนไป ทุกบทจบด้วยของที่เล่นได้จริงในเกม

## ✅ สิ่งที่เรียนผ่านมาแล้ว (อ่านจากโค้ด + git log)

| แนวคิด | อยู่ในไฟล์ไหน |
|--------|----------------|
| ตัวแปร + `[SerializeField]` โชว์ใน Inspector | ทุกไฟล์ |
| `enum` + `switch` — สีลูกสนุกเกอร์ 8 สี | `Ball.cs` |
| method, `GetComponent`, Renderer | `Ball.cs` |
| Singleton (`static instance`) + Property (get/set) | `GameManger.cs` |
| `Rigidbody`, `Collider`, Physic Material, `AddForce` | `Ball.cs`, `Ball.prefab` |
| Vector3 + Ray จากเมาส์ + Plane.Raycast | `CueController.cs` |
| Trigger Collider + `OnTriggerEnter` | `Pocket.cs` |
| `OnCollisionEnter` — ตรวจว่าลูกขาวโดนลูกไหนก่อน | `Ball.cs` |
| `List<T>` + loop จัดการลูกทั้งโต๊ะ | `GameManger.cs` |
| state อย่างง่าย — เทิร์น / ต้องแทงแดงหรือสี / ฟาวล์ | `GameManger.cs` |
| Canvas + UI Text + Button + `SceneManager.LoadScene` | `UIManager.cs` |
| หลายซีน + Build Settings + `SceneManager.LoadScene("ชื่อซีน")` | `MainMenuManager.cs`, `MainMenu.unity` |
| เปิด/ปิด UI ด้วย `SetActive` + `Mathf.Sin` ทำหัวข้อเต้น | `MainMenuManager.cs` |

## 🎮 วิธีเล่น

- เริ่มที่ซีน `MainMenu` กด PLAY เพื่อเข้าเกม, HOW TO PLAY ดูวิธีเล่น, QUIT ออกเกม
- เลื่อนเมาส์เพื่อเล็ง ไม้คิวจะหมุนตาม
- กดเมาส์ซ้ายค้าง = สะสมแรง (ดูแถบสีเหลืองมุมซ้ายล่าง) ปล่อยเพื่อแทง
- แทงแดงก่อน แล้วสลับเป็นลูกสี ลูกสีลงแล้ววางกลับที่เดิมถ้ายังมีแดงเหลือ
- ฟาวล์ (ลูกขาวลง / แทงผิดลูก / แทงไม่โดนอะไรเลย) = คู่ต่อสู้ได้ 4 แต้ม แล้วสลับตา
- ลูกหมดโต๊ะ = จบเกม ปุ่ม Restart โหลดซีนใหม่

## 🧩 โครงสร้างไฟล์

| ไฟล์ | หน้าที่ |
|------|---------|
| `Ball.cs` | สี/แต้มของลูก, จำจุดวางเดิม, ยิงลูก, รายงานลูกแรกที่ลูกขาวชน |
| `Pocket.cs` | trigger 6 หลุม ส่งลูกที่ตกให้ GameManger |
| `GameManger.cs` | วางลูก 22 ใบตอน Start, นับแต้ม, กติกา, เทิร์น, จบเกม |
| `CueController.cs` | เล็งด้วยเมาส์, กดค้างวัดแรง, ขยับไม้คิว |
| `UIManager.cs` | คะแนน, เทิร์น, ข้อความ, แถบแรง, จอจบเกม |
| `MainMenuManager.cs` | หน้าเมนูหลัก — ปุ่ม PLAY / HOW TO PLAY / QUIT, แผงวิธีเล่น |

## 🔧 ต่อยอดได้อีก

- เสียงลูกกระทบ + เอฟเฟกต์ตอนลงหลุม
- บังคับลำดับลูกสีตอนแดงหมด (ตอนนี้แทงสีไหนก่อนก็ได้)
- กล้องหมุนรอบโต๊ะ / มุมมองจากหลังลูกขาว
- Build เป็น .exe ส่งให้เพื่อนเล่น
- ปุ่ม "กลับเมนู" ในจอจบเกม (`SceneManager.LoadScene("MainMenu")`)

---

## โน้ตจากผู้สอน

- คลาส `GameManger` สะกดผิด (ที่ถูกคือ `GameManager`) — ยังคงชื่อเดิมไว้เพราะซีนลิงก์อยู่กับ GUID ของไฟล์นี้ เปลี่ยนชื่อคลาสเมื่อไรต้องเปลี่ยนพร้อมกันทั้งไฟล์และซีน
- `Test.cs` ลบไปแล้วตอนจบบท 1
- ไฟล์ `Assets/Materials/BallColor/*.mat` ยังไม่ได้ใช้ เพราะ `Ball.cs` ตั้งสีด้วย `rd.material.color` ถ้าอยากลดจำนวน material instance ค่อยเปลี่ยนไปใช้ `.mat` ทั้ง 8 ไฟล์แทน
- ลำดับซีนใน **File > Build Profiles/Build Settings** สำคัญ: `MainMenu` ต้องเป็นซีนแรก (index 0) เกมถึงจะเปิดมาที่เมนู ส่วน `SampleScene` เป็น index 1 — ปุ่ม Restart ใน `UIManager` ใช้ `GetActiveScene().buildIndex` เลยไม่พังตอนสลับลำดับ
- `MainMenuManager.QuitGame()` ใน Editor จะสั่งหยุด Play Mode ให้ (`Application.Quit()` ไม่มีผลตอนกด Play) ต้อง Build เป็น .exe ถึงจะออกโปรแกรมจริง
