
# Shelter Helper

Keep track of your shelter's resources, manage the animals that stay under your care. 
## Lessons Learned

It's important to separate the functionalities early, untangling dependencies takes much more time and effort than learning how to give them their own "boxes" to "live in". I prefer to use such metaphors because visualizing the parts of code as little animals with homes and jobs helps me to understand how the parts of my app work and what is required for them to provide the expected results.

Setting a button's type to "button" prevents it from triggering a form validation when it's pressed.
## Documentation

`IHttpClientFactory` is used because there's a need for differently configured HttpClient instances.

