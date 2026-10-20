using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MembersController : Controller
    {
        private readonly MemberRepository _memberRepo;

        public MembersController(MemberRepository memberRepo)
        {
            _memberRepo = memberRepo;
        }

        private bool IsValidMember(Member member)
        {
            if (string.IsNullOrWhiteSpace(member.Name))
                ModelState.AddModelError("Name", "กรุณากรอกชื่อ");
            else if (member.Name.Length > 50)
                ModelState.AddModelError("Name", "ชื่อยาวไม่เกิน 50 ตัวอักษร");

            if (string.IsNullOrWhiteSpace(member.Email))
                ModelState.AddModelError("Email", "กรุณากรอกอีเมล");
            else if (member.Email.Length > 100)
                ModelState.AddModelError("Email", "อีเมลยาวไม่เกิน 100 ตัวอักษร");

            if (string.IsNullOrWhiteSpace(member.Password))
                ModelState.AddModelError("Password", "กรุณากรอกรหัสผ่าน");
            else if (member.Password.Length > 50)
                ModelState.AddModelError("Password", "รหัสผ่านยาวไม่เกิน 50 ตัวอักษร");

            if (!string.IsNullOrWhiteSpace(member.Phone) && member.Phone.Length > 12)
                ModelState.AddModelError("Phone", "เบอร์โทรยาวไม่เกิน 12 ตัวอักษร");

            return ModelState.ErrorCount == 0;
        }

        public IActionResult Index(string? keyword)
        {
            try
            {
                List<Member> members = _memberRepo.Search(keyword);
                ViewBag.Keyword = keyword;
                return View(members);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "โหลดข้อมูลสมาชิกไม่สำเร็จ: " + ex.Message;
                return View(new List<Member>());
            }
        }

        public IActionResult Create()
        {
            return View(new Member());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Member member)
        {
            if (!IsValidMember(member))
            {
                return View(member);
            }

            try
            {
                int newId = _memberRepo.Insert(member);
                TempData["Message"] = $"เพิ่มข้อมูลสำเร็จ (รหัสสมาชิกใหม่คือ {newId})";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "เพิ่มข้อมูลไม่สำเร็จ: " + ex.Message);
                return View(member);
            }
        }

        public IActionResult Edit(int id)
        {
            Member? member = _memberRepo.SearchById(id);

            if (member == null)
            {
                TempData["Error"] = "ไม่พบสมาชิกรหัสนี้";
                return RedirectToAction("Index");
            }

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Member member)
        {
            if (!IsValidMember(member))
            {
                return View(member);
            }

            try
            {
                int rows = _memberRepo.Update(member);
                TempData["Message"] = $"แก้ไขข้อมูลสำเร็จ (จำนวนแถวที่กระทบคือ {rows})";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "แก้ไขข้อมูลไม่สำเร็จ: " + ex.Message);
                return View(member);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _memberRepo.Delete(id);
                TempData["Message"] = "ลบข้อมูลสำเร็จ";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "ลบข้อมูลไม่สำเร็จ: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}