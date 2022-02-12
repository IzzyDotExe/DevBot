var floater  = document.getElementById("side-nav");
var footer = document.getElementsByTagName("footer")[0];
console.log(floater)

function getRectFoot(){
    var rect = footer.getBoundingClientRect();
    return rect.top;
}

function getRectFloat(){
    var rect = floater.getBoundingClientRect();
    return rect.top;
}

function checkOffset() {

    if((getRectFloat() + document.body.scrollTop) + floater.offsetHeight >= (getRectFoot() + document.body.scrollTop) - 10)
        floater.style.position = 'absolute';
    if(document.body.scrollTop + window.innerHeight < (getRectFoot() + document.body.scrollTop))
        floater.style.position = 'fixed'; // restore when you scroll up
  

  
}

document.addEventListener("scroll", function(){
  checkOffset();
});