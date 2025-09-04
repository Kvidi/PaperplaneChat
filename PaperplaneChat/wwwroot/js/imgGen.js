
document.getElementById("prompt").addEventListener('keydown', function (e) {
    if (e.key !== 'Enter') {
        this.classList.remove("error");
        this.placeholder = "Add descriptive text here...";
    }

    const promptText = this.value.trim();
    if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault(); // Prevent new line        
        document.getElementById("generateBtn").click();
    }
});

document.getElementById("generateBtn").addEventListener('click', function () {
    const genBtn = document.getElementById("generateBtn");
    const prompt = document.getElementById("prompt")
    const promptText = prompt.value.trim();

    if (promptText.length === 0) {
        prompt.value = "";
        prompt.placeholder = "Please enter a prompt!";
        prompt.classList.add("error");
        return;
    }

    genBtn.disabled = true;
    generateBtn.innerHTML = "Please wait...";

    GenerateImage();
});

function GenerateImage() {
    const promptText = document.getElementById("prompt").value;
    const genBtn = document.getElementById("generateBtn");

    document.getElementById("img-div").innerHTML = "";
    document.getElementById("bot-loading").classList.remove('d-none');

    genBtn.disabled = true;

    $.ajax({
        type: 'post',
        url: '/Home/GenerateImage',
        dataType: 'json',
        data: { text: promptText },
        success: function (imageUrl) {
            document.getElementById("bot-loading").classList.add('d-none');
            document.getElementById("img-div").innerHTML = `<img src="${imageUrl}" alt="Generated Image" class="img-fluid" />`;

            genBtn.disabled = false;
            genBtn.innerHTML = "Generate";
        },
        error: function () {
            document.getElementById("bot-loading").classList.add('d-none');
            document.getElementById("img-div").innerHTML = `<p class="text-warning">Error generating image. Please try again.</p>`;
            genBtn.disabled = false;
            genBtn.innerHTML = "Generate";
        }
    });
}

