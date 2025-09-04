
document.addEventListener('DOMContentLoaded', function () {

    let firstMessageSent = false;
    const userInput = document.getElementById('userInput');
    const chatForm = document.getElementById('chatForm');
    const chatMessages = document.getElementById('chatMessages');    
    const headerText = document.getElementById('headerText');
    const layout = document.getElementById('layout');
    const paperplaneIcon = document.querySelector('.flyAway-icon');
    const btnScrollToBottomChat = document.getElementById('btnScrollToBottomChat');     
    const chatMessagesWrapper = document.getElementById('chatMessagesWrapper'); 
    const banner = document.getElementById('flyAwayBanner');
    const bannerString = document.getElementById('flyAwayBannerString');

    // Check if first message was sent in the session
    if (sessionStorage.getItem('firstMessageSent') === 'true') {
        applyFirstMessageSentStyles();
        firstMessageSent = true;
    }

    // Submit form on Enter key press
    userInput.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault(); // Prevent new line
            chatForm.dispatchEvent(new Event('submit', { cancelable: true , bubbles: true })); 
        }
    });

    // Listen for scrolling on chatMessages, then show/hide the button
    chatMessagesWrapper.addEventListener('scroll', function () {
        // Check if the chat is scrolled to near the bottom
        const threshold = 50;
        const atBottom = chatMessagesWrapper.scrollHeight - chatMessagesWrapper.scrollTop - chatMessagesWrapper.clientHeight < threshold;
        btnScrollToBottomChat.style.display = atBottom ? 'none' : 'block';
    });

    // Click event to scroll to the bottom
    btnScrollToBottomChat.addEventListener('click', function () {
        if (chatMessagesWrapper) {
            chatMessagesWrapper.scrollTo({
                top: chatMessagesWrapper.scrollHeight,
                behavior: 'smooth'
            });
        }
        btnScrollToBottomChat.style.display = 'none';
    });

    // Initialize Bootstrap tooltips
    const newTooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        newTooltips.forEach(function (tooltipEl) {
            new bootstrap.Tooltip(tooltipEl, {
                trigger: "hover"
            });
        });

    // Scroll to the top of a specific message
    document.addEventListener('click', function (event) {
        const btn = event.target.closest('.btn-scroll-to-top-msg');
        if (btn) {
            const message = btn.closest('.message'); // Find the closest message container
                        
            if (message && chatMessagesWrapper) {
                // Scroll to the message's position within the chatMessagesWrapper container
                const offsetTop = message.offsetTop - chatMessagesWrapper.offsetTop;
                chatMessagesWrapper.scrollTo({
                    top: offsetTop,
                    behavior: 'smooth'
                });
            }
        }
    });

    // Submit form on form submit
    chatForm.addEventListener('submit', function (e) {
        e.preventDefault(); // Prevent form submission

        // Capture the user input before clearing
        const messageValue = userInput.value.trim();
        if (!messageValue) return;

        // Apply styles and clear user input immediately on submit
        if (!firstMessageSent) {
            applyFirstMessageSentStyles();
            firstMessageSent = true;
            sessionStorage.setItem('firstMessageSent', 'true');
        }
        userInput.value = '';

        // Add user message
        $('#chatMessages').append(`
            <div class="message user">
                <p>${messageValue}</p>
            </div>
        `);

        // Append a loading spinner to indicate the AI is processing
        $('#chatMessages').append(`
          <div id="bot-loading" class="message bot justify-content-start">
             <p>
                <span class="spinner-border spinner-border-sm" role="status"></span>
                <span>AI is processing...</span>
             </p>
          </div>
        `);
        chatMessagesWrapper.scrollTop = chatMessagesWrapper.scrollHeight;

        $.ajax({
            type: 'POST',
            url: chatForm.action,
            data: { UserMessage: messageValue }, // send the captured value
            success: function (response) {
                // Remove the loading spinner
                $('#bot-loading').remove();
                                
                // Add bot message
                $('#chatMessages').html(response);    

                // Reinitialize tooltips on the newly inserted elements
                const newTooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
                newTooltips.forEach(function (tooltipEl) {
                    new bootstrap.Tooltip(tooltipEl, {
                        trigger: "hover"
                    });
                });
            }
        });
    });
        
    function applyFirstMessageSentStyles() {
        headerText.classList.add('d-none');
        chatMessagesWrapper.classList.remove('d-none');
        banner.classList.remove('d-none');
        bannerString.classList.remove('d-none');
        banner.classList.add('fly-away-banner');
        bannerString.classList.add('fly-away-banner-string');
        paperplaneIcon.classList.add('fly-away');
        layout.classList.remove('initial-state');
        layout.classList.add('final-state');        
    }
});

