import { useState, useRef, useEffect, useCallback } from 'react';
import { motion } from 'framer-motion';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import Header from './layout/Header';
import Hero from './Hero';
import ModulesSection from './modules/ModulesSection';
import Consulting from './Consulting';
import PricingCalculator from './PricingCalculator';
import Contact from './Contact';
import Footer from './layout/Footer';

const sections = [
  { id: 'hero', component: Hero, name: 'Inicio' },
  { id: 'modules', component: ModulesSection, name: 'Módulos' },
  { id: 'consulting', component: Consulting, name: 'Consultoría' },
  { id: 'pricing', component: PricingCalculator, name: 'Precios' },
  { id: 'contact', component: Contact, name: 'Contacto' },
  { id: 'footer', component: Footer, name: 'Final' },
];

const JEGASolutionsLanding = () => {
  const [activeSection, setActiveSection] = useState(0);
  const [isDesktop, setIsDesktop] = useState(true);
  const containerRef = useRef(null);
  const sectionRefs = useRef([]);

  useEffect(() => {
    const mediaQuery = window.matchMedia('(min-width: 1024px)');
    setIsDesktop(mediaQuery.matches);

    const handleResize = () => setIsDesktop(mediaQuery.matches);
    mediaQuery.addEventListener('change', handleResize);
    return () => mediaQuery.removeEventListener('change', handleResize);
  }, []);

  const scrollToSection = useCallback(
    index => {
      const section = sectionRefs.current[index];
      if (!section) return;

      if (isDesktop) {
        const container = containerRef.current;
        if (container) {
          // Use scrollTo for more reliable horizontal scrolling
          setActiveSection(index); // Set active section immediately on scroll command
          container.scrollTo({
            left: section.offsetLeft,
            behavior: 'smooth',
          });
        }
      } else {
        // Fallback for mobile
        setActiveSection(index); // Set active section immediately on scroll command
        section.scrollIntoView({
          behavior: 'smooth',
          block: 'start',
        });
      }
    },
    [isDesktop]
  );

  useEffect(() => {
    const container = containerRef.current;
    if (isDesktop && container) {
      let isScrolling = false;
      let scrollTimeout;

      const handleWheel = e => {
        e.preventDefault();
        if (isScrolling) return;

        isScrolling = true;

        if (e.deltaY > 0) {
          // Scrolling down
          nextSection();
        } else {
          // Scrolling up
          prevSection();
        }

        // Debounce to prevent rapid section changes
        scrollTimeout = setTimeout(() => {
          isScrolling = false;
        }, 800); // Adjust time as needed for feel
      };

      container.addEventListener('wheel', handleWheel, { passive: false });
      return () => {
        container.removeEventListener('wheel', handleWheel);
        clearTimeout(scrollTimeout);
      };
    }
  }, [isDesktop, scrollToSection]); // Add scrollToSection to dependencies

  const nextSection = () => {
    setActiveSection(prevIndex => {
      const nextIndex = Math.min(prevIndex + 1, sections.length - 1);
      scrollToSection(nextIndex);
      return nextIndex;
    });
  };

  const prevSection = () => {
    setActiveSection(prevIndex => {
      const nextIndex = Math.max(prevIndex - 1, 0);
      scrollToSection(nextIndex);
      return nextIndex;
    });
  };

  return (
    <div className="w-screen bg-gray-50 m-0 p-0 lg:h-screen lg:overflow-hidden">
      {/* Header Component */}
      <Header activeSection={activeSection} scrollToSection={scrollToSection} />

      <div
        ref={containerRef}
        className="flex flex-col lg:flex-row lg:h-full lg:w-full lg:overflow-x-scroll lg:snap-x lg:snap-mandatory"
        // The duplicate style attribute was removed. The one below is kept.
        style={{ scrollBehavior: 'smooth' }}
      >
        {sections.map((Section, index) => (
          <div
            key={Section.id}
            id={Section.id}
            ref={el => (sectionRefs.current[index] = el)}
            className="w-full flex-shrink-0 lg:h-screen lg:min-h-screen lg:snap-start lg:overflow-y-auto"
          >
            <Section.component
              onContactClick={() => scrollToSection(4)}
              onDemoClick={() => scrollToSection(1)}
              onScrollToTop={() =>
                isDesktop
                  ? scrollToSection(0)
                  : window.scrollTo({ top: 0, behavior: 'smooth' })
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
                ? 'bg-jega-blue-600 scale-125'
                : 'bg-gray-300 hover:bg-gray-400'
            }`}
            aria-label={`Ir a la sección ${section.name}`}
          ></button>
        ))}
      </div>
    </div>
  );
};

export default JEGASolutionsLanding;
