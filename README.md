# DigitizedDallet
> If you have any question or want to reach out, ping [@sferhah](http://github.com/sferhah) or send an email to sferhah@gmail.com
 
This project involves the digitization and [structuring](https://en.wikipedia.org/wiki/Data_modeling) of the **J.M. DALLET Kabyle-French dictionary**, a posthumous work authored by [Jean-Marie DALLET](https://www.wikidata.org/wiki/Q117833370) (1909 – 1972). The dictionary was completed and published in 1982 by [Madeleine ALLAIN](https://www.wikidata.org/wiki/Q25691511), Jacques LANFRY, and Pieter REESINK, preserving the original author's sole attribution.

All the data is consolidated into a single file named : [dictionary.json](https://github.com/sferhah/DigitizedDallet/blob/master/DigitizedDallet/wwwroot/dictionary.json). As the file extension implies, the process goes beyond mere digitization; it encompasses a thorough data modeling effort.

This repository contains the source code for a web-based version of the JSON file, accessible at: [digitized-dallet.com](http://digitized-dallet.com).

# Digitization & data modelling
## Digitization
While the OCR process performed admirably in transcribing the French text, it encountered difficulties with the Kabyle text, necessitating manual input. This task was particularly challenging due to the subpar print quality, which employed italics for the Kabyle text and introduced the potential for confusion between characters such as 'l' and 't,' 'a' and 'u,' among others. I had it reviewed and rectified by a native speaker.

## Data modelling
The data modeling process was partially automated using a variety of unsaved scripts, which may have led to certain instances of misstructured data. I am currently in the process of refining it, so it has not been finalized yet, and there is a possibility that the data model may undergo further changes.

# Website
The [web-based interface](http://digitized-dallet.com) is built on the ASP.NET Core MVC framework and remains a work in progress. While its visual design may not be particularly polished, this is primarily due to the limited time available for actual coding, with approximately 99.99% of the effort dedicated to the digitization process.

The debug configuration enables the modification of the JSON file.
