﻿using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
	[TestFixture]
	public class ObjectPrinterAcceptanceTests
	{
		public Person CurrPerson;
		
		[SetUp]
		public void SetUp()
		{
			CurrPerson = new Person {Name = "Alex", Age = 19, Height = 191.5, Birthdate = new DateTime(2015, 7, 20, 18, 30, 25)};
		}

		[Test]
		public void CorrectPropertyExcluding()
		{
			const string expected = "Person\n" +
			                        "\tId == Guid\n" +
			                        "\tName == Alex\n" +
			                        "\tHeight == 191,5\n" +
			                        "\tBirthdate == 20.07.2015 18:30:25\n";
			CurrPerson.PrintToString(s => s.ExcludeProperty(p => p.Age))
				.Should().Be(expected);
		}
		
		[Test]
		public void CorrectTypeExcluding()
		{
			const string expected = "Person\n" +
			                        "\tName == Alex\n" +
			                        "\tHeight == 191,5\n" +
			                        "\tAge == 19\n" +
			                        "\tBirthdate == 20.07.2015 18:30:25\n";
			ObjectPrinter.For<Person>()
				.ExcludeType<Guid>()
				.PrintToString(CurrPerson)
				.Should().Be(expected);
		}

		[Test]
		public void AlternativeSerializingForType()
		{
			const string expected = "Person\n" +
			                        "\tId == Guid\n" +
			                        "\tName == Alex\n" +
			                        "\tHeight == 191,5\n" +
			                        "\tAge == test\n" +
			                        "\tBirthdate == 20.07.2015 18:30:25\n";
			ObjectPrinter.For<Person>()
				.Printing<int>()
				.Using(i => "test")
				.PrintToString(CurrPerson)
				.Should().Be(expected);
		}
		
		[Test]
		public void AlternativeSerializingForProperty()
		{
			const string expected = "Person\n" +
			                        "\tId == Guid\n" +
			                        "\tName == Alex\n" +
			                        "\tHeight == 191,5\n" +
			                        "\tAge == test\n" +
			                        "\tBirthdate == 20.07.2015 18:30:25\n";
			ObjectPrinter.For<Person>()
				.Printing(p => p.Age)
				.Using(i => "test")
				.PrintToString(CurrPerson)
				.Should().Be(expected);
		}
		
		[Test]
		public void TrimStringProperty()
		{
			const string expected = "Person\n" +
			                        "\tId == Guid\n" +
			                        "\tName == Al\n" +
			                        "\tHeight == 191,5\n" +
			                        "\tAge == 19\n" +
			                        "\tBirthdate == 20.07.2015 18:30:25\n";
			ObjectPrinter.For<Person>()
				.Printing(p => p.Name)
				.TrimTo(2)
				.PrintToString(CurrPerson)
				.Should().Be(expected);
		}

		[Test]
		public void CorrectCultureUsing()
		{
			const double height = 191.5;
			var date = new DateTime(2015, 7, 20, 18, 30, 25);
			var culturicalHeight = height.ToString(new CultureInfo("ru-RU"));
			var culturicalDate = date.ToString(new CultureInfo("ru-RU"));
			var expected = "Person\n" +
			               "\tId == Guid\n" +
			               "\tName == Alex\n" +
			               $"\tHeight == {culturicalHeight}\n" +
			               "\tAge == 19\n" +
			               $"\tBirthdate == {date}\n";
			ObjectPrinter.For<Person>()
				.Printing<int>()
				.Using(new CultureInfo("ru-RU"))
				.PrintToString(CurrPerson)
				.Should().Be(expected);
		}
		
		[Test]
		public void Demo()
		{

			var printer = ObjectPrinter.For<Person>()
				//1. Исключить из сериализации свойства определенного типа
				.ExcludeType<Guid>()
				//2. Указать альтернативный способ сериализации для определенного типа
				.Printing<int>()
				.Using(i => i.ToString())
				//3. Для числовых типов указать культуру
				.Printing<int>()
				.Using(CultureInfo.CurrentCulture)
				//4. Настроить сериализацию конкретного свойства
				.Printing(p => p.Age)
				.Using(age => age.ToString())
				//5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
				.Printing(p => p.Name)
				.TrimTo(3)
				//6. Исключить из сериализации конкретного свойства
				.ExcludeProperty(p => p.Age);
            string s1 = printer.PrintToString(CurrPerson);

			//7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию	
			string s2 = CurrPerson.PrintToString();
			//8. ...с конфигурированием
			string s3 = CurrPerson.PrintToString(s => s.ExcludeProperty(p => p.Age));
		}
	}
}