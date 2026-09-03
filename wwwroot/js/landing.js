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

    // ── Mega menu — hover with delay, correct positioning ──
    var megaCloseTimer = null;
    var mainNavEl = document.getElementById('mainNav');

    function positionMegaMenu(menu) {
        // Positioned via CSS relative to container
    }

    document.querySelectorAll('.nav-dropdown').forEach(function (dropdown) {
        var menu = dropdown.querySelector('.nav-mega-menu--full');
        if (!menu) return;

        function openMenu() {
            clearTimeout(megaCloseTimer);
            // close others
            document.querySelectorAll('.nav-mega-menu--full.is-open').forEach(function (m) {
                if (m !== menu) m.classList.remove('is-open');
            });
            positionMegaMenu(menu);
            menu.classList.add('is-open');
        }

        function closeMenu() {
            megaCloseTimer = setTimeout(function () {
                menu.classList.remove('is-open');
            }, 180);
        }

        dropdown.addEventListener('mouseenter', openMenu);
        dropdown.addEventListener('mouseleave', closeMenu);
        menu.addEventListener('mouseenter', function () { clearTimeout(megaCloseTimer); });
        menu.addEventListener('mouseleave', closeMenu);
    });

    // Close mega menu on outside click
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.nav-dropdown') && !e.target.closest('.nav-mega-menu--full')) {
            document.querySelectorAll('.nav-mega-menu--full.is-open').forEach(function (m) {
                m.classList.remove('is-open');
            });
        }
    });

    // Reposition on scroll (since navbar is sticky)
    window.addEventListener('scroll', function () {
        document.querySelectorAll('.nav-mega-menu--full.is-open').forEach(function (menu) {
            positionMegaMenu(menu);
        });
    }, { passive: true });

    // ── Hero Carousel Arrow Click Handler ──────
    document.addEventListener('click', function (e) {
        const prevBtn = e.target.closest('.hero-arrow-left, .hero-nav-prev');
        const nextBtn = e.target.closest('.hero-arrow-right, .hero-nav-next');
        const carouselEl = document.getElementById('homeHeroCarousel');
        if (!carouselEl) return;

        if (prevBtn) {
            e.preventDefault();
            e.stopPropagation();
            if (typeof bootstrap !== 'undefined' && bootstrap.Carousel) {
                bootstrap.Carousel.getOrCreateInstance(carouselEl).prev();
            } else if (typeof jQuery !== 'undefined') {
                jQuery(carouselEl).carousel('prev');
            }
        } else if (nextBtn) {
            e.preventDefault();
            e.stopPropagation();
            if (typeof bootstrap !== 'undefined' && bootstrap.Carousel) {
                bootstrap.Carousel.getOrCreateInstance(carouselEl).next();
            } else if (typeof jQuery !== 'undefined') {
                jQuery(carouselEl).carousel('next');
            }
        }
    });

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
    // UNIQUE FLOATING CONNECT US HUB
    // ══════════════════════════════════════════
    var connectTrigger   = document.getElementById('caConnectTrigger');
    var connectMenu      = document.getElementById('caConnectMenu');
    var formDrawer       = document.getElementById('caFormDrawer');
    var connectBackdrop  = document.getElementById('caConnectBackdrop');
    var openFormBtn      = document.getElementById('caOpenFormBtn');
    var drawerBack       = document.getElementById('caDrawerBack');
    var drawerClose      = document.getElementById('caDrawerClose');
    var callbackForm     = document.getElementById('caCallbackForm');
    var successBox       = document.getElementById('cbSuccessBox');

    function openConnectMenu() {
        if (!connectMenu) return;
        closeDrawer();
        connectMenu.classList.add('active');
        connectMenu.setAttribute('aria-hidden', 'false');
        if (connectTrigger) {
            connectTrigger.classList.add('active');
            connectTrigger.setAttribute('aria-expanded', 'true');
        }
        if (connectBackdrop) connectBackdrop.classList.add('active');
    }

    function closeConnectMenu() {
        if (!connectMenu) return;
        connectMenu.classList.remove('active');
        connectMenu.setAttribute('aria-hidden', 'true');
        if (connectTrigger && !isDrawerOpen()) {
            connectTrigger.classList.remove('active');
            connectTrigger.setAttribute('aria-expanded', 'false');
        }
        if (!isDrawerOpen() && connectBackdrop) {
            connectBackdrop.classList.remove('active');
        }
    }

    function openDrawer() {
        if (!formDrawer) return;
        closeConnectMenu();
        formDrawer.classList.add('active');
        formDrawer.setAttribute('aria-hidden', 'false');
        if (connectTrigger) {
            connectTrigger.classList.add('active');
            connectTrigger.setAttribute('aria-expanded', 'true');
        }
        if (connectBackdrop) connectBackdrop.classList.add('active');
        setTimeout(function () {
            var first = formDrawer.querySelector('input, select');
            if (first) first.focus();
        }, 150);
    }

    function closeDrawer() {
        if (!formDrawer) return;
        formDrawer.classList.remove('active');
        formDrawer.setAttribute('aria-hidden', 'true');
        if (connectTrigger && !isMenuOpen()) {
            connectTrigger.classList.remove('active');
            connectTrigger.setAttribute('aria-expanded', 'false');
        }
        if (!isMenuOpen() && connectBackdrop) {
            connectBackdrop.classList.remove('active');
        }
    }

    function closeAllConnect() {
        closeConnectMenu();
        closeDrawer();
        if (connectTrigger) {
            connectTrigger.classList.remove('active');
            connectTrigger.setAttribute('aria-expanded', 'false');
        }
        if (connectBackdrop) connectBackdrop.classList.remove('active');
    }

    function isMenuOpen() {
        return connectMenu && connectMenu.classList.contains('active');
    }

    function isDrawerOpen() {
        return formDrawer && formDrawer.classList.contains('active');
    }

    if (connectTrigger) {
        connectTrigger.addEventListener('click', function () {
            if (isMenuOpen() || isDrawerOpen()) {
                closeAllConnect();
            } else {
                openConnectMenu();
            }
        });
    }

    if (openFormBtn) {
        openFormBtn.addEventListener('click', function (e) {
            e.preventDefault();
            openDrawer();
        });
    }

    if (drawerBack) {
        drawerBack.addEventListener('click', function () {
            closeDrawer();
            openConnectMenu();
        });
    }

    if (drawerClose) {
        drawerClose.addEventListener('click', closeAllConnect);
    }

    if (connectBackdrop) {
        connectBackdrop.addEventListener('click', closeAllConnect);
    }

    // "Consult Now" navbar button opens drawer directly
    document.querySelectorAll('.btn-nav-cta').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            openDrawer();
        });
    });

    // Close on ESC
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && (isMenuOpen() || isDrawerOpen())) {
            closeAllConnect();
        }
    });

    // Form Validation & Handling
    if (callbackForm) {
        callbackForm.addEventListener('submit', function (e) {
            e.preventDefault();
            var valid = true;

            var name    = document.getElementById('cbName');
            var phone   = document.getElementById('cbPhone');
            var service = document.getElementById('cbService');

            ['cbNameErr', 'cbPhoneErr', 'cbServiceErr'].forEach(function (id) {
                var el = document.getElementById(id);
                if (el) el.classList.remove('show');
            });
            [name, phone, service].forEach(function (el) {
                if (el) el.classList.remove('error');
            });

            if (!name || !name.value.trim() || name.value.trim().length < 2) {
                var nErr = document.getElementById('cbNameErr');
                if (nErr) nErr.classList.add('show');
                if (name) name.classList.add('error');
                valid = false;
            }

            var phoneVal = phone ? phone.value.replace(/\s/g, '') : '';
            if (!phoneVal || !/^[+0-9]{10,15}$/.test(phoneVal)) {
                var pErr = document.getElementById('cbPhoneErr');
                if (pErr) pErr.classList.add('show');
                if (phone) phone.classList.add('error');
                valid = false;
            }

            if (!service || !service.value) {
                var sErr = document.getElementById('cbServiceErr');
                if (sErr) sErr.classList.add('show');
                if (service) service.classList.add('error');
                valid = false;
            }

            if (!valid) return;

            var submitBtn = document.getElementById('cbSubmitBtn');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Submitting...';
            }

            // Simulated request / API post
            setTimeout(function () {
                callbackForm.querySelectorAll('.ca-form-field, .ca-submit-btn').forEach(function (el) {
                    el.style.display = 'none';
                });
                if (successBox) successBox.style.display = 'block';

                setTimeout(function () {
                    closeAllConnect();
                    callbackForm.reset();
                    callbackForm.querySelectorAll('.ca-form-field, .ca-submit-btn').forEach(function (el) {
                        el.style.display = '';
                    });
                    if (successBox) successBox.style.display = 'none';
                    if (submitBtn) {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = '<span>Request Callback</span><i class="fas fa-paper-plane ms-2"></i>';
                    }
                }, 3500);
            }, 600);
        });
    }

})();
