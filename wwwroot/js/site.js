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