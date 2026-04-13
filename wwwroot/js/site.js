// ── Toast System ───────────────────────────────────────────
function showToast(message, type = 'info') {
    const container = document.getElementById('toastContainer');
    if (!container) return;

    const icons = { success: '✓', error: '✕', info: 'ℹ', warning: '⚠' };

    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `<span style="font-size:15px">${icons[type] || 'ℹ'}</span> ${message}`;

    container.appendChild(toast);

    setTimeout(() => {
        toast.style.animation = 'slideInRight .25s ease reverse forwards';
        setTimeout(() => toast.remove(), 250);
    }, 4000);
}

// ── Auto-trigger toasts from TempData ─────────────────────
document.addEventListener('DOMContentLoaded', function () {
    const body = document.body;

    const success = body.getAttribute('data-toast-success');
    const error   = body.getAttribute('data-toast-error');

    if (success) showToast(success, 'success');
    if (error)   showToast(error,   'error');

    // ── Dark Mode Toggle ───────────────────────────────────────
const themeToggle = document.getElementById('themeToggle');
const themeIcon   = document.getElementById('themeIcon');
const html        = document.documentElement;

const savedTheme = localStorage.getItem('bm-theme') || 'dark';
html.setAttribute('data-theme', savedTheme);
updateThemeIcon(savedTheme);

if (themeToggle) {
    themeToggle.addEventListener('click', function () {
        const current = html.getAttribute('data-theme');
        const next    = current === 'dark' ? 'light' : 'dark';
        html.setAttribute('data-theme', next);
        localStorage.setItem('bm-theme', next);
        updateThemeIcon(next);
    });
}

function updateThemeIcon(theme) {
    if (!themeIcon) return;
    if (theme === 'light') {
        themeIcon.classList.add('is-light');
    } else {
        themeIcon.classList.remove('is-light');
    }
}

    // ── Sidebar Toggle ─────────────────────────────────────
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar       = document.getElementById('sidebar');

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function () {
            sidebar.classList.toggle('open');
        });
    }

    // ── Form Loading States ────────────────────────────────
    document.querySelectorAll('form').forEach(function (form) {
        form.addEventListener('submit', function () {
            const btn = form.querySelector('[type="submit"]');
            if (btn) {
                btn.disabled = true;
                btn.textContent = 'Please wait...';
            }
        });
    });

    // ── Student ID field toggle on CreateUser ──────────────
    const roleSelect    = document.getElementById('Role');
    const studentIdWrap = document.getElementById('studentIdWrapper');

    if (roleSelect && studentIdWrap) {
        function toggleStudentId() {
            studentIdWrap.style.display =
                roleSelect.value === 'Student' ? 'block' : 'none';
        }
        roleSelect.addEventListener('change', toggleStudentId);
        toggleStudentId();
    }
});


document.addEventListener("DOMContentLoaded", () => {
    // 1. Theme Toggle Logic
    const themeToggleBtn = document.getElementById('theme-toggle');
    const currentTheme = localStorage.getItem('theme') || 'dark';
    
    document.documentElement.setAttribute('data-theme', currentTheme);
    updateThemeIcon(currentTheme);

    if (themeToggleBtn) {
        themeToggleBtn.addEventListener('click', () => {
            let theme = document.documentElement.getAttribute('data-theme');
            let newTheme = theme === 'dark' ? 'light' : 'dark';
            
            document.documentElement.setAttribute('data-theme', newTheme);
            localStorage.setItem('theme', newTheme);
            updateThemeIcon(newTheme);
        });
    }

    function updateThemeIcon(theme) {
        if (!themeToggleBtn) return;
        themeToggleBtn.innerHTML = theme === 'dark' ? '☀️' : '🌙'; 
    }

    // 2. Toast Notification System
    const body = document.querySelector('body');
    const successMsg = body.getAttribute('data-toast-success');
    const errorMsg = body.getAttribute('data-toast-error');

    if (successMsg) showToast(successMsg, 'success');
    if (errorMsg) showToast(errorMsg, 'error');

    // 3. Form Submission Spinner
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function() {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...';
            }
        });
    });

    // 4. Student ID Field Toggle (For Admin Create User)
    const roleSelect = document.getElementById('RoleSelect');
    const studentIdContainer = document.getElementById('StudentIdContainer');
    
    if (roleSelect && studentIdContainer) {
        roleSelect.addEventListener('change', (e) => {
            if (e.target.value === 'Student') {
                studentIdContainer.style.display = 'block';
            } else {
                studentIdContainer.style.display = 'none';
            }
        });
    }
});

// Global function to call from anywhere
window.showToast = function(message, type = 'info') {
    const container = document.getElementById('toast-container') || createToastContainer();
    
    const toast = document.createElement('div');
    toast.className = `custom-toast ${type}`;
    toast.innerText = message;
    
    container.appendChild(toast);
    
    // Trigger animation
    setTimeout(() => toast.classList.add('show'), 10);
    
    // Auto remove
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 4000);
};

function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toast-container';
    document.body.appendChild(container);
    return container;
}