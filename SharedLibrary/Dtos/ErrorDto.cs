﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    //Tüm Api lerde response model olarak ErrorDto kullanacağız. O sebeple SharedLibrary içine aldık.
    public class ErrorDto
    {
        public ErrorDto()
        {
            Errors = new List<string>();
        }

        public ErrorDto(string error,bool isShow)
        {
            Errors.Add(error);
            IsShow = isShow;
        }

        public ErrorDto(List<string> errors,bool isShow)
        {
            Errors=errors;
            IsShow=isShow;
        }

        //Dışarıdan set edilememesi için private set yaptık. Sadece constructor ile doldurulacak.
        public List<string> Errors{get;private set;}
        //Hatayı kullanıcıya gösterip göstermemek için "IsShow" property'si ekledik.
        public bool IsShow { get;private set; }




    }
}