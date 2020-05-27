# Merit Badge Blue Card Exporter
This web application allows you to conveniently populate BSA merit badge blue cards based on files that contain the relevant data and export them to a PDF. The PDF can then be printed double-sided to produce blue cards. All that is left to do is for the counselor to sign the completed blue cards!

## Features of the application
- Able to fill out the blue card with the following if provided and as appropriate:
  - Student name, address, unit type, unit number, district, council
  - Merit badge name, requirements, completion date of the requirements, and counselor initials if appropriate
  - Counselor name, address, telephone
  - Completion date if appropriate
  - Remarks
- Able to sort the PDF export using the following options
  - Use the provided student order
  - Sort by the student first name
  - Sort by the student last name
  - Sort by the student unit number
- Able to validate a file before exporting blue cards
- Documented file specifications and template files
- Able to be run locally so your data does not leave your machine

## Who might use this?
- Summer camps
- Merit Badge Colleges/Universities/Festivals
- Individual merit badge counselors
- Anyone else who wants to save time and already has their class info electronically documented

## How to Run
- Get the application by downloading the contents of the publish folder.
- Execute the .exe file
- Navigate to the localhost site that shows in your favorite browser
- Enjoy!

## Technical Information
- An [Export Specification](https://github.com/jonliew/Merit-Badge-Blue-Card-Exporter/blob/master/BlueCardExporter/wwwroot/files/Export_Specifications.pdf) was written to help define the details of the files. This could also be used by other developers to help transfer merit badge class data.
- Yes, this is a .NET Core Razor Pages application that can be run locally or published to the Internet. Why? Because I wanted this to be compatible with Windows, Linux, and MacOS.
