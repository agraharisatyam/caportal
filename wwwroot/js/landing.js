/* =============================================
   CACampus — Landing Page JS
   ============================================= */

(function () {
    'use strict';

    // ── Navbar scroll effect ──────────────────
    const nav = document.getElementById('mainNav');
    if (nav) {
        const onScroll = () => nav.classList.toggle('scrolled', window.scrollY > 40);
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
    // EXPERT CONSULTATION POPUP
    // ══════════════════════════════════════════
    const overlay    = document.getElementById('consultOverlay');
    const trigger    = document.getElementById('consultTrigger');
    const closeBtn   = document.getElementById('consultClose');
    const form       = document.getElementById('consultForm');
    const successMsg = document.getElementById('consultSuccess');

    function openPopup() {
        if (!overlay) return;
        overlay.classList.add('active');
        overlay.setAttribute('aria-hidden', 'false');
        document.body.style.overflow = 'hidden';
        // Focus first input for accessibility
        setTimeout(function () {
            const first = overlay.querySelector('input, select');
            if (first) first.focus();
        }, 100);
    }

    function closePopup() {
        if (!overlay) return;
        overlay.classList.remove('active');
        overlay.setAttribute('aria-hidden', 'true');
        document.body.style.overflow = '';
    }

    // Open on floating button
    if (trigger) trigger.addEventListener('click', openPopup);

    // Open on "Consult Now" navbar button
    document.querySelectorAll('.btn-nav-cta').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            openPopup();
        });
    });

    // Close on X button
    if (closeBtn) closeBtn.addEventListener('click', closePopup);

    // Close on overlay click (outside popup)
    if (overlay) {
        overlay.addEventListener('click', function (e) {
            if (e.target === overlay) closePopup();
        });
    }

    // Close on ESC key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closePopup();
    });

    // Auto-open after 8 seconds (first visit only)
    if (!sessionStorage.getItem('consultShown')) {
        setTimeout(function () {
            openPopup();
            sessionStorage.setItem('consultShown', '1');
        }, 8000);
    }

    // Form validation & submit
    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            let valid = true;

            const name    = document.getElementById('consultName');
            const phone   = document.getElementById('consultPhone');
            const service = document.getElementById('consultService');

            // Reset errors
            document.querySelectorAll('.consult-error').forEach(function (el) {
                el.classList.remove('visible');
            });
            [name, phone, service].forEach(function (el) {
                el.classList.remove('error');
            });

            // Validate name
            if (!name.value.trim() || name.value.trim().length < 2) {
                document.getElementById('nameError').classList.add('visible');
                name.classList.add('error');
                valid = false;
            }

            // Validate phone
            const phoneVal = phone.value.replace(/\s/g, '');
            if (!phoneVal || !/^[+0-9]{10,15}$/.test(phoneVal)) {
                document.getElementById('phoneError').classList.add('visible');
                phone.classList.add('error');
                valid = false;
            }

            // Validate service
            if (!service.value) {
                document.getElementById('serviceError').classList.add('visible');
                service.classList.add('error');
                valid = false;
            }

            if (!valid) return;

            // Success
            form.querySelectorAll('.consult-field, .consult-submit').forEach(function (el) {
                el.style.display = 'none';
            });
            if (successMsg) {
                successMsg.style.display = 'block';
            }

            // Auto close after 3s
            setTimeout(function () {
                closePopup();
                // Reset form
                form.reset();
                form.querySelectorAll('.consult-field, .consult-submit').forEach(function (el) {
                    el.style.display = '';
                });
                if (successMsg) successMsg.style.display = 'none';
            }, 3000);
        });
    }

})();
