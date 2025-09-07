import { useState, useRef, useEffect, useCallback } from "react";
import { motion } from "framer-motion";
import { ChevronLeft, ChevronRight } from "lucide-react";
import Header from "./layout/Header";
import Hero from "./Hero";
import ModulesSection from "./modules/ModulesSection";
import Consulting from "./Consulting";
import PricingCalculator from "./PricingCalculator";
import Contact from "./Contact";
import Footer from "./layout/Footer";

const sections = [
  { id: "hero", component: Hero, name: "Inicio" },
  { id: "modules", component: ModulesSection, name: "Módulos" },
  { id: "consulting", component: Consulting, name: "Consultoría" },
  { id: "pricing", component: PricingCalculator, name: "Precios" },
  { id: "contact", component: Contact, name: "Contacto" },
  { id: "footer", component: Footer, name: "Final" },
];

const JEGASolutionsLanding = () => {
  const [activeSection, setActiveSection] = useState(0);
  const [isDesktop, setIsDesktop] = useState(true);
  const containerRef = useRef(null);
  const sectionRefs = useRef([]);

  useEffect(() => {
    const mediaQuery = window.matchMedia("(min-width: 1024px)");
    setIsDesktop(mediaQuery.matches);

    const handleResize = () => setIsDesktop(mediaQuery.matches);
    mediaQuery.addEventListener("change", handleResize);
    return () => mediaQuery.removeEventListener("change", handleResize);
  }, []);

  useEffect(() => {
    const container = containerRef.current;
    if (isDesktop && container) {
      let scrollTimeout;
      const handleWheel = (e) => {
        // Prioritize vertical scroll to convert to horizontal.
        // This avoids conflicts with horizontal trackpad swipes.
        if (Math.abs(e.deltaY) > Math.abs(e.deltaX)) {
          e.preventDefault();

          // Temporarily disable snap for smooth wheel scrolling.
          // The browser will try to snap to the nearest section after each small scroll
          // which creates a jerky effect.
          if (container.style.scrollSnapType !== "none") {
            container.style.scrollSnapType = "none";
          }

          container.scrollLeft += e.deltaY;

          // Re-enable snap after a short delay of no scrolling
          clearTimeout(scrollTimeout);
          scrollTimeout = setTimeout(() => {
            container.style.scrollSnapType = "x mandatory";
          }, 150);
        }
      };

      container.addEventListener("wheel", handleWheel, { passive: false });

      return () => {
        container.removeEventListener("wheel", handleWheel);
        if (scrollTimeout) {
          clearTimeout(scrollTimeout);
        }
      };
    }
  }, [isDesktop]);

  const scrollToSection = useCallback(
    (index) => {
      const section = sectionRefs.current[index];
      if (!section) return;

      if (isDesktop) {
        const container = containerRef.current;
        if (container) {
          // Use scrollTo for more reliable horizontal scrolling
          container.scrollTo({
            left: section.offsetLeft,
            behavior: "smooth",
          });
        }
      } else {
        // Fallback for mobile
        section.scrollIntoView({
          behavior: "smooth",
          block: "start",
        });
      }
    },
    [isDesktop]
  );

  const nextSection = () => {
    const nextIndex = Math.min(activeSection + 1, sections.length - 1);
    scrollToSection(nextIndex);
  };

  const prevSection = () => {
    const prevIndex = Math.max(activeSection - 1, 0);
    scrollToSection(prevIndex);
  };

  useEffect(() => {
    // Don't run observer if refs are not ready
    if (!sectionRefs.current.length) return;
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            const index = sectionRefs.current.findIndex(
              (ref) => ref === entry.target
            );
            if (index !== -1) {
              setActiveSection(index);
            }
          }
        });
      },
      { threshold: 0.6, root: isDesktop ? containerRef.current : null }
    );

    sectionRefs.current.forEach((ref) => {
      if (ref) observer.observe(ref);
    });

    return () => {
      sectionRefs.current.forEach((ref) => {
        if (ref) observer.unobserve(ref);
      });
      observer.disconnect();
    };
  }, [isDesktop]);

  return (
    <div className="w-screen bg-gray-50 m-0 p-0 lg:h-screen lg:overflow-hidden">
      {/* Header Component */}
      <Header activeSection={activeSection} scrollToSection={scrollToSection} />

      <div
        ref={containerRef}
        className="flex flex-col lg:flex-row lg:h-full lg:w-full lg:overflow-x-scroll lg:snap-x lg:snap-mandatory"
        // This is important to make wheel scroll immediate, while button clicks can be smooth.
        style={{ scrollBehavior: isDesktop ? "auto" : "smooth" }}
      >
        {sections.map((Section, index) => (
          <div
            key={Section.id}
            ref={(el) => (sectionRefs.current[index] = el)}
            className="w-full flex-shrink-0 lg:h-screen lg:min-h-screen lg:snap-start lg:overflow-y-auto"
          >
            <Section.component
              onContactClick={() => scrollToSection(4)}
              onDemoClick={() => scrollToSection(3)}
              onScrollToTop={() =>
                isDesktop
                  ? scrollToSection(0)
                  : window.scrollTo({ top: 0, behavior: "smooth" })
              }
            />
          </div>
        ))}
      </div>

      {isDesktop && activeSection > 0 && (
        <motion.button
          onClick={prevSection}
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          exit={{ opacity: 0, x: -20 }}
          className="fixed left-4 top-1/2 transform -translate-y-1/2 z-40 bg-white/80 hover:bg-white shadow-lg rounded-full p-3 transition-all duration-300 hover:scale-110 hidden lg:flex"
        >
          <ChevronLeft className="w-6 h-6 text-gray-700" />
        </motion.button>
      )}
      {isDesktop && activeSection < sections.length - 1 && (
        <motion.button
          onClick={nextSection}
          initial={{ opacity: 0, x: 20 }}
          animate={{ opacity: 1, x: 0 }}
          exit={{ opacity: 0, x: 20 }}
          className="fixed right-4 top-1/2 transform -translate-y-1/2 z-40 bg-white/80 hover:bg-white shadow-lg rounded-full p-3 transition-all duration-300 hover:scale-110 hidden lg:flex"
        >
          <ChevronRight className="w-6 h-6 text-gray-700" />
        </motion.button>
      )}

      <div className="fixed right-8 top-1/2 transform -translate-y-1/2 z-40 space-y-2 hidden lg:block">
        {sections.map((section, index) => (
          <button
            key={section.id}
            onClick={() => scrollToSection(index)}
            className={`w-3 h-3 rounded-full transition-all duration-300 ${
              activeSection === index
                ? "bg-jega-blue-600 scale-125"
                : "bg-gray-300 hover:bg-gray-400"
            }`}
            aria-label={`Ir a la sección ${section.name}`}
          ></button>
        ))}
      </div>
    </div>
  );
};

export default JEGASolutionsLanding;
