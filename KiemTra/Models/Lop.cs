﻿namespace KiemTra.Models
{
    public class Ward
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public string DivisionType { get; set; }
        public string Codename { get; set; }
    }

    public class District
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public string DivisionType { get; set; }
        public string Codename { get; set; }
        public int ProvinceCode { get; set; }
        public List<Ward> Wards { get; set; }
    }

    public class Province
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public string DivisionType { get; set; }
        public string Codename { get; set; }
        public int PhoneCode { get; set; }
        public List<District> Districts { get; set; }
    }


}
