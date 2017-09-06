<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>BinaryCalendar</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

PublicHolidays.DeleteAllOnSubmit(PublicHolidays);
SubmitChanges();

//PublicHoliday christmas = new PublicHoliday()
//{
//	PublicHolidayId = Guid.NewGuid(),
//	CountryCode = "ZA",
//	CountryName = "South Africa",
//	EventName = "Christmas Day",
//	DateIdentifier = "2016-12-25",
//	Year = 2016,
//	Month = 12,
//	Day = 25,
//	HolidayDate = new DateTime(2016, 12, 25),
//	DateCreated = DateTime.Now
//};
//PublicHolidays.InsertOnSubmit(christmas);
SubmitChanges();

PublicHolidays.Dump();