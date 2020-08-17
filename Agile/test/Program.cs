using Agile.BaseLib.Extension;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(DateTime.Now.AddMinutes(1));
            //List<Student> list = new List<Student>() {
            //new Student(){ Id =1,Age = 18,Name = "haha1"},
            //new Student(){ Id =2,Age = 18,Name = "haha2"},
            //new Student(){ Id =3,Age = 18,Name = "haha3"}
            //};
            //List<Student> list2 = new List<Student>() {
            //new Student(){ Id =1,Age = 18,Name = "haha1"},
            //new Student(){ Id =2,Age = 18,Name = "haha2"},
            //new Student(){ Id =3,Age = 18,Name = "haha3"}
            //};

            //list.AddRange(list2);

            //foreach (var item in list)
            //{
            //    Console.WriteLine(item.Id);
            //}

            //ConstantExpression _constExp = System.Linq.Expressions.Expression.Constant("aaa",typeof(string));

            GetLambdaStr<Student>(x => x.Name == "abc");


        }


        private static string GetLambdaStr<T>(Expression<Func<T, bool>> expression)
        {
            //解析表达式
            //return new Analysis().AnalysisExpression(expression);
            return null;
        }
    }
}
