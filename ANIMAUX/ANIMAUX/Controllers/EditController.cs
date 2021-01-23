﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using ANIMAUX.Models;
using GemBox.Document;
using GemBox.Spreadsheet;

namespace ANIMAUX.Controllers
{
    public class EditController : Controller
    {
        AnimauxEntities entities = new AnimauxEntities();
        public ActionResult Publication(int id)
        {
            var pub = entities.publications.Where(x => x.id == id).FirstOrDefault();
            ViewBag.animals = entities.animals;

            var month = pub.added_date.Month < 10 ? "0" + pub.added_date.Month.ToString() : pub.added_date.Month.ToString();
            var day = pub.added_date.Day < 10 ? "0" + pub.added_date.Day.ToString() : pub.added_date.Day.ToString();
            var year = pub.added_date.Year;

            ViewBag.added = year + "-" + month + "-" + day;

            ViewBag.id = pub.id;
            ViewBag.url = pub.photo;
            ViewBag.city = pub.city;
            ViewBag.type = pub.type;

            ViewBag.animal = entities.animals.Where(x => x.passport_number == pub.animal_id).FirstOrDefault().name;

            return View();
        }

        [HttpPost]
        public ActionResult Publication(FormCollection form)
        {
            var pubId = int.Parse(form["id"]);
            var pub = entities.publications.Where(x => x.id == pubId).FirstOrDefault();
            pub.photo = form["newUrlPhoto"];
            pub.city = form["newUrlCity"];
            pub.type = form["type"] == "lost" ? "l" : "f";
            var animalId = form["newAnimal"];

            pub.animal_id = entities.animals.Where(x => x.name == animalId).FirstOrDefault().passport_number;
            entities.SaveChanges();

            return Redirect(Url.Action("Publications", "Home"));
        }

        public ActionResult Card(int id)
        {
            var pub = entities.cards.Where(x => x.id == id).FirstOrDefault();

            var month = pub.date_added.Month < 10 ? "0" + pub.date_added.Month.ToString() : pub.date_added.Month.ToString();
            var day = pub.date_added.Day < 10 ? "0" + pub.date_added.Day.ToString() : pub.date_added.Day.ToString();
            var year = pub.date_added.Year;

            ViewBag.added = year + "-" + month + "-" + day;

            ViewBag.id = pub.id;
            ViewBag.name = pub.animals.name;
            ViewBag.sex = pub.animals.sex;
            ViewBag.districts = entities.districts;

            month = pub.animals.birth_date.Month < 10 ? "0" + pub.animals.birth_date.Month.ToString() : pub.animals.birth_date.Month.ToString();
            day = pub.animals.birth_date.Day < 10 ? "0" + pub.animals.birth_date.Day.ToString() : pub.animals.birth_date.Day.ToString();
            year = pub.date_added.Year;

            ViewBag.birthDate = year + "-" + month + "-" + day;

            ViewBag.animal = entities.animals.Where(x => x.passport_number == pub.animal_id).FirstOrDefault().name;

            ViewBag.dist = pub.districts.id;
            return View();
        }

        [HttpPost]
        public ActionResult Card(FormCollection form)
        {
            var pubId = int.Parse(form["id"]);
            var pub = entities.cards.Where(x => x.id == pubId).FirstOrDefault();
            pub.animals.name = form["newName"];
            pub.animals.sex = form["sex"];
            pub.animals.birth_date = Convert.ToDateTime(form["newBirthDate"]);
            pub.district_id = int.Parse(form["newDist"]);

            entities.SaveChanges();

            return Redirect(Url.Action("Registry", "Home"));
        }


        public ActionResult ExportWord()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            // Load template document.
            DocumentModel document = new DocumentModel();

            string resp = "";

            foreach (var card in entities.cards)
            {
                resp += "id: " + card.id + ", Организация: " + card.organisations.name
                    + ", Район: " + card.districts.name + ", Животное:" + card.animals.name +
                    ", Добавлено:" + card.date_added.ToString();
            }

            document.Sections.Add(
                new Section(document,
                new Paragraph(document,
                new Run(document, resp))));

            // Create document in specified format (PDF, DOCX, etc.) and
            // stream (download) it to client's browser.
            document.Save(this.Response, $"export{".docx"}");

            return Redirect(Url.Action("Registry", "Home"));
        }

        public ActionResult ExportExcel()
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            // Create Excel file.
            var workbook = new ExcelFile();

            // Add a new worksheet to the Excel file.
            var worksheet = workbook.Worksheets.Add("Публикации");

            worksheet.Cells["A1"].Value = "id";
            worksheet.Cells["B1"].Value = "Добавлено";
            worksheet.Cells["C1"].Value = "Фотка";
            worksheet.Cells["D1"].Value = "Город";
            worksheet.Cells["E1"].Value = "Животное";
            worksheet.Cells["F1"].Value = "Тип";

            int i = 2;
            foreach (publications pub in entities.publications)
            {
                worksheet.Cells["A" + i].Value = pub.id;
                worksheet.Cells["B" + i].Value = pub.added_date.ToString();
                worksheet.Cells["C" + i].Value = pub.photo;
                worksheet.Cells["D" + i].Value = pub.city;
                worksheet.Cells["E" + i].Value = pub.animals.name;
                worksheet.Cells["F" + i].Value = pub.type == "l" ? "Потеряно" : "Найдено";
                i++;
            }

            // Create document in specified format (PDF, DOCX, etc.) and
            // stream (download) it to client's browser.
            workbook.Save(this.Response, $"export{".xlsx"}");

            return Redirect(Url.Action("Publications", "Home"));
        }

    }
}