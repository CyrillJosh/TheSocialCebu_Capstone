const html = document.getElementById("htmlPage");

const theme = localStorage.getItem("Theme");

if(theme != null)
{
    html.setAttribute("data-bs-theme", theme);
}

$(document).ready(function(){
    const html = document.getElementById("htmlPage");
    const chkbox = document.getElementById("checkbox");
    chkbox.checked = theme == "dark" ?  true : false;
    chkbox.addEventListener("change", () => {
        console.log(chkbox.checked);
        if(chkbox.checked){
            html.setAttribute("data-bs-theme", "dark");
            localStorage.setItem("Theme", "dark");
        }
        else {
            html.setAttribute("data-bs-theme", "light");
            localStorage.setItem("Theme", "light");
        }
    })
});
