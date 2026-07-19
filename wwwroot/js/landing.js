/* =============================================
   CACampus — Landing Page JS
   ============================================= */

(function () {
    'use strict';

    // ── Navbar scroll effect ──────────────────
    const nav = document.getElementById('mainNav');
    if (nav) {
        const onScroll = () => nav.classList.toggle('scrolled', window.scrollY > 60);
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll();
    }

    // ── Smooth scroll for anchor links ────────
    document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');
            if (href === '#') return;
            const target = document.querySelector(href);
            if (target) {
                e.preventDefault();
                const offset = 110;
                const top = target.getBoundingClientRect().top + window.scrollY - offset;
                window.scrollTo({ top: top, behavior: 'smooth' });
                const mobileNav = document.getElementById('mobileNav');
                if (mobileNav && mobileNav.classList.contains('show')) {
                    const bsCollapse = bootstrap.Collapse.getInstance(mobileNav);
                    if (bsCollapse) bsCollapse.hide();
                }
            }
        });
    });

    // ── Scroll reveal ─────────────────────────
    const revealEls = document.querySelectorAll('.reveal');
    if ('IntersectionObserver' in window) {
        const observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });
        revealEls.forEach(function (el) { observer.observe(el); });
    } else {
        revealEls.forEach(function (el) { el.classList.add('visible'); });
    }

    // ── Topbar dropdown keyboard accessibility ─
    document.querySelectorAll('.topbar-dropdown').forEach(function (drop) {
        const btn = drop.querySelector('.topbar-btn');
        if (btn) {
            btn.addEventListener('focus', function () { btn.setAttribute('aria-expanded', 'true'); });
            btn.addEventListener('blur', function () {
                setTimeout(function () { btn.setAttribute('aria-expanded', 'false'); }, 200);
            });
        }
    });

    // ── Hero v2 stat counter animation ────────
    function animateCounter(el, target, prefix, suffix) {
        let start = 0;
        const duration = 1600;
        const step = (target / duration) * 16;
        const timer = setInterval(function () {
            start += step;
            if (start >= target) {
                el.textContent = prefix + target + suffix;
                clearInterval(timer);
            } else {
                el.textContent = prefix + Math.floor(start) + suffix;
            }
        }, 16);
    }

    const heroV2 = document.querySelector('.hero-v2');
    if (heroV2) {
        let done = false;
        const obs = new IntersectionObserver(function (entries) {
            if (entries[0].isIntersecting && !done) {
                done = true;
                const nums = document.querySelectorAll('.hero-v2-stat-num');
                const data = [
                    { value: 12,  prefix: '', suffix: 'K+' },
                    { value: 50,  prefix: '', suffix: 'K+' },
                    { value: 200, prefix: '', suffix: '+' },
                    { value: 98,  prefix: '', suffix: '%' },
                ];
                nums.forEach(function (el, i) {
                    if (data[i]) animateCounter(el, data[i].value, data[i].prefix, data[i].suffix);
                });
                obs.disconnect();
            }
        }, { threshold: 0.3 });
        obs.observe(heroV2);
    }

    // ══════════════════════════════════════════
    // FLOATING CONSULT WIDGET
    // ══════════════════════════════════════════
    var floatTrigger  = document.getElementById('floatTriggerBtn');
    var floatPanel    = document.getElementById('floatFormPanel');
    var floatClose    = document.getElementById('floatFormClose');
    var floatForm     = document.getElementById('floatConsultForm');
    var floatSuccess  = document.getElementById('floatSuccess');
    var floatBtnIcon  = document.getElementById('floatBtnIcon');
    var isOpen        = false;

    function openWidget() {
        if (!floatPanel) return;
        isOpen = true;
        floatPanel.classList.add('open');
        floatPanel.setAttribute('aria-hidden', 'false');
        floatTrigger.classList.add('open');
        floatTrigger.setAttribute('aria-expanded', 'true');
        floatBtnIcon.className = 'fas fa-times float-btn-icon';
        // focus first input
        setTimeout(function () {
            var first = floatPanel.querySelector('input, select');
            if (first) first.focus();
        }, 150);
    }

    function closeWidget() {
        if (!floatPanel) return;
        isOpen = false;
        floatPanel.classList.remove('open');
        floatPanel.setAttribute('aria-hidden', 'true');
        floatTrigger.classList.remove('open');
        floatTrigger.setAttribute('aria-expanded', 'false');
        floatBtnIcon.className = 'fas fa-comments float-btn-icon';
    }

    if (floatTrigger) {
        floatTrigger.addEventListener('click', function () {
            isOpen ? closeWidget() : openWidget();
        });
    }

    if (floatClose) {
        floatClose.addEventListener('click', closeWidget);
    }

    // "Consult Now" navbar button also opens it
    document.querySelectorAll('.btn-nav-cta').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            openWidget();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    });

    // Close on ESC
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && isOpen) closeWidget();
    });

    // Auto-open after 10 seconds (once per session)
    if (!sessionStorage.getItem('floatShown')) {
        setTimeout(function () {
            openWidget();
            sessionStorage.setItem('floatShown', '1');
        }, 10000);
    }

    // Form validation & submit
    if (floatForm) {
        floatForm.addEventListener('submit', function (e) {
            e.preventDefault();
            var valid = true;

            var name    = document.getElementById('fcName');
            var phone   = document.getElementById('fcPhone');
            var service = document.getElementById('fcService');

            // reset
            ['fcNameErr','fcPhoneErr','fcServiceErr'].forEach(function (id) {
                var el = document.getElementById(id);
                if (el) el.classList.remove('show');
            });
            [name, phone, service].forEach(function (el) {
                if (el) el.classList.remove('error');
            });

            if (!name || !name.value.trim() || name.value.trim().length < 2) {
                document.getElementById('fcNameErr').classList.add('show');
                name.classList.add('error');
                valid = false;
            }

            var phoneVal = phone ? phone.value.replace(/\s/g, '') : '';
            if (!phoneVal || !/^[+0-9]{10,15}$/.test(phoneVal)) {
                document.getElementById('fcPhoneErr').classList.add('show');
                phone.classList.add('error');
                valid = false;
            }

            if (!service || !service.value) {
                document.getElementById('fcServiceErr').classList.add('show');
                service.classList.add('error');
                valid = false;
            }

            if (!valid) return;

            // Show success
            floatForm.querySelectorAll('.float-field, .float-submit').forEach(function (el) {
                el.style.display = 'none';
            });
            if (floatSuccess) floatSuccess.style.display = 'block';

            // Reset & close after 3s
            setTimeout(function () {
                closeWidget();
                floatForm.reset();
                floatForm.querySelectorAll('.float-field, .float-submit').forEach(function (el) {
                    el.style.display = '';
                });
                if (floatSuccess) floatSuccess.style.display = 'none';
            }, 3000);
        });
    }

})();
