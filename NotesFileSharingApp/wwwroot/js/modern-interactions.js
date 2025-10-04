/* =============================================
   CLEAN AESTHETIC NOTES APP - SIMPLIFIED INTERACTIONS
   Focus on usability and subtle animations
   ============================================= */

$(document).ready(function() {
    
    // =============================================
    // CLEAN THEME TOGGLE SYSTEM
    // =============================================
    
    // Check for saved theme or default to light
    const savedTheme = localStorage.getItem('theme') || 'light';
    $('html').attr('data-theme', savedTheme);
    
    // Create clean theme toggle button
    if ($('.theme-toggle').length === 0) {
        $('body').append(`
            <button class="theme-toggle" id="theme-toggle" title="Toggle Dark/Light Theme">
                <i class="bi bi-moon-fill"></i>
            </button>
        `);
    }
    
    // Update theme toggle icon
    function updateThemeIcon() {
        const currentTheme = $('html').attr('data-theme');
        const icon = currentTheme === 'dark' ? 'bi-sun-fill' : 'bi-moon-fill';
        $('#theme-toggle i').attr('class', `bi ${icon}`);
    }
    
    updateThemeIcon();
    
    // Clean theme toggle
    $('#theme-toggle').click(function() {
        const currentTheme = $('html').attr('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        
        $('html').attr('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);
        updateThemeIcon();
    });
    
    // =============================================
    // SUBTLE COUNTER ANIMATION
    // =============================================
    
    // Simple counter animation for stats
    function animateCounters() {
        $('.stat-number').each(function() {
            const $this = $(this);
            const target = parseInt($this.text()) || 0;
            
            if (target > 0 && target <= 1000) { // Only animate reasonable numbers
                $this.text('0');
                $({ counter: 0 }).animate({ counter: target }, {
                    duration: 1500,
                    easing: 'swing',
                    step: function() {
                        $this.text(Math.ceil(this.counter));
                    },
                    complete: function() {
                        $this.text(target);
                    }
                });
            }
        });
    }
    
    // Trigger counter animation when visible
    const statsObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                animateCounters();
                statsObserver.unobserve(entry.target);
            }
        });
    });
    
    $('.stat-number').each(function() {
        statsObserver.observe(this);
    });
    
    // =============================================
    // CLEAN FORM ENHANCEMENTS
    // =============================================
    
    // Better form focus states
    $('.form-control').focus(function() {
        $(this).closest('.form-group, .mb-3').addClass('focused');
    }).blur(function() {
        $(this).closest('.form-group, .mb-3').removeClass('focused');
    });
    
    // =============================================
    // SUBTLE LOADING STATES
    // =============================================
    
    // Clean loading state for forms
    $('form').submit(function() {
        const $form = $(this);
        const $submitBtn = $form.find('[type="submit"]');
        const originalText = $submitBtn.text();
        
        $submitBtn.prop('disabled', true);
        $submitBtn.html('<i class="bi bi-hourglass-split me-2"></i>Processing...');
        
        // Reset after a reasonable timeout if no redirect occurs
        setTimeout(() => {
            $submitBtn.prop('disabled', false);
            $submitBtn.text(originalText);
        }, 10000);
    });
    
    // =============================================
    // CLEAN SCROLL ANIMATIONS
    // =============================================
    
    // Subtle fade-in for cards
    const scrollObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                $(entry.target).addClass('fade-in');
                scrollObserver.unobserve(entry.target);
            }
        });
    }, { threshold: 0.1, rootMargin: '50px' });
    
    // Observe important elements
    $('.card, .alert').each(function() {
        scrollObserver.observe(this);
    });
    
    // =============================================
    // CLEAN FAB (Floating Action Button)
    // =============================================
    
    // Context-aware FAB
    function createCleanFAB() {
        const currentPage = window.location.pathname.toLowerCase();
        let fabContent = '';
        
        if (currentPage.includes('/notes') && !currentPage.includes('/create')) {
            fabContent = '<a href="/Notes/Create" class="fab" title="Create New Note"><i class="bi bi-plus"></i></a>';
        } else if (currentPage.includes('/files') && !currentPage.includes('/upload')) {
            fabContent = '<a href="/Files/Upload" class="fab" title="Upload File"><i class="bi bi-upload"></i></a>';
        } else if (currentPage === '/' || currentPage.includes('/home')) {
            fabContent = '<a href="/Notes/Create" class="fab" title="Create Note"><i class="bi bi-plus"></i></a>';
        }
        
        if (fabContent) {
            $('body').append(fabContent);
        }
    }
    
    createCleanFAB();
    
    // =============================================
    // CLEAN NOTIFICATIONS
    // =============================================
    
    // Auto-dismiss alerts after reasonable time
    $('.alert').each(function() {
        const $alert = $(this);
        setTimeout(() => {
            $alert.fadeOut(300, function() {
                $alert.remove();
            });
        }, 5000);
        
        // Pause auto-dismiss on hover
        $alert.hover(
            function() { clearTimeout(this.dismissTimer); },
            function() {
                this.dismissTimer = setTimeout(() => {
                    $alert.fadeOut(300, function() { $alert.remove(); });
                }, 2000);
            }
        );
    });
    
    // =============================================
    // COPY TO CLIPBOARD FUNCTIONALITY
    // =============================================
    
    // Global copy function for notes/content
    window.copyToClipboard = function(text, button) {
        navigator.clipboard.writeText(text).then(() => {
            showCleanNotification('Content copied to clipboard!', 'success');
            
            if (button) {
                const $btn = $(button);
                const originalContent = $btn.html();
                $btn.html('<i class="bi bi-check me-1"></i> Copied!');
                $btn.removeClass('btn-outline-info').addClass('btn-success');
                
                setTimeout(() => {
                    $btn.html(originalContent);
                    $btn.removeClass('btn-success').addClass('btn-outline-info');
                }, 2000);
            }
        }).catch(() => {
            showCleanNotification('Failed to copy content', 'danger');
        });
    };
    
    // =============================================
    // CLEAN SEARCH ENHANCEMENTS
    // =============================================
    
    // Simple search input enhancements
    $('input[type="search"], input[name*="search"]').on('input', function() {
        const $this = $(this);
        const query = $this.val();
        
        if (query.length > 0) {
            $this.addClass('has-content');
        } else {
            $this.removeClass('has-content');
        }
    });
    
    // =============================================
    // ACCESSIBILITY IMPROVEMENTS
    // =============================================
    
    // Keyboard navigation for custom elements
    $('.fab, .theme-toggle').attr('tabindex', '0').keydown(function(e) {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            $(this)[0].click();
        }
    });
    
    // Focus management for modals
    $('.modal').on('shown.bs.modal', function() {
        $(this).find('input:first, button:first').focus();
    });
    
    console.log('? Clean theme loaded successfully!');
});

// =============================================
// CLEAN UTILITY FUNCTIONS
// =============================================

// Simple, clean notification system
window.showCleanNotification = function(message, type = 'info', duration = 3000) {
    const notification = $(`
        <div class="alert alert-${type} alert-dismissible fade show position-fixed" 
             style="top: 20px; right: 20px; z-index: 1055; max-width: 350px; box-shadow: var(--shadow-lg);">
            <i class="bi bi-${type === 'success' ? 'check-circle' : type === 'danger' ? 'exclamation-circle' : 'info-circle'} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `);
    
    $('body').append(notification);
    
    setTimeout(() => {
        notification.fadeOut(300, function() {
            $(this).remove();
        });
    }, duration);
};

// Simple loading state management
window.setLoadingState = function(element, loading = true) {
    const $el = $(element);
    if (loading) {
        $el.prop('disabled', true);
        $el.data('original-html', $el.html());
        $el.html('<i class="bi bi-hourglass-split me-2"></i>Loading...');
    } else {
        $el.prop('disabled', false);
        $el.html($el.data('original-html') || 'Submit');
    }
};

// Debounced resize handler for performance
let resizeTimer;
$(window).resize(function() {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(() => {
        // Handle responsive adjustments here if needed
        console.log('Window resized');
    }, 250);
});