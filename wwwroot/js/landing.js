/* =============================================
   CACampus — Landing Page JS
   ============================================= */

(function () {
    'use strict';

    // ── Navbar scroll effect ──────────────────
    const nav = document.getElementById('mainNav');
    if (nav) {
        const onScroll = () => {
            nav.classList.toggle('scrolled', window.scrollY > 40);
        };
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll(); // run once on load
    }

    // ── Smooth scroll for anchor links ────────
    document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
        anchor.addEventListener('click', function (e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                const offset = 80; // navbar height
                const top = target.getBoundingClientRect().top + window.scrollY - offset;
                window.scrollTo({ top: top, behavior: 'smooth' });

                // Close mobile menu if open
                const mobileNav = document.getElementById('mobileNav');
                if (mobileNav && mobileNav.classList.contains('show')) {
                    const bsCollapse = bootstrap.Collapse.getInstance(mobileNav);
                    if (bsCollapse) bsCollapse.hide();
                }
            }
        });
    });

    // ── Scroll reveal animation ───────────────
    const revealEls = document.querySelectorAll('.reveal');

    if ('IntersectionObserver' in window) {
        const observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12 });

        revealEls.forEach(function (el) {
            observer.observe(el);
        });
    } else {
        // Fallback: show all immediately
        revealEls.forEach(function (el) { el.classList.add('visible'); });
    }

    // ── Animated number counter in hero stats ─
    function animateCounter(el, target, suffix) {
        let start = 0;
        const duration = 1800;
        const step = (target / duration) * 16;
        const timer = setInterval(function () {
            start += step;
            if (start >= target) {
                el.textContent = target + suffix;
                clearInterval(timer);
            } else {
                el.textContent = Math.floor(start) + suffix;
            }
        }, 16);
    }

    const stats = [
        { selector: '.hero-stats .hero-stat-item:nth-child(1) .hero-stat-number', value: 12, suffix: 'K+' },
        { selector: '.hero-stats .hero-stat-item:nth-child(2) .hero-stat-number', value: 98, suffix: '%' },
        { selector: '.hero-stats .hero-stat-item:nth-child(3) .hero-stat-number', value: 50, suffix: 'K+' }
    ];

    let countersStarted = false;
    const heroSection = document.querySelector('.hero-section');

    if (heroSection) {
        const counterObserver = new IntersectionObserver(function (entries) {
            if (entries[0].isIntersecting && !countersStarted) {
                countersStarted = true;
                stats.forEach(function (s) {
                    const el = document.querySelector(s.selector);
                    if (el) animateCounter(el, s.value, s.suffix);
                });
                counterObserver.disconnect();
            }
        }, { threshold: 0.3 });
        counterObserver.observe(heroSection);
    }

})();
