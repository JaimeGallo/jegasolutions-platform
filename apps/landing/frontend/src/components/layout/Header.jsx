import React, { useState } from 'react';
import { motion } from 'framer-motion';
import { Menu, X } from 'lucide-react';

const Header = ({ activeSection, scrollToSection }) => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const navItems = [
    { label: 'Módulos', sectionIndex: 1 },
    { label: 'Consultoría', sectionIndex: 2 },
    { label: 'Contacto', sectionIndex: 4 },
  ];

  const handleNavClick = index => {
    scrollToSection(index);
    setIsMenuOpen(false);
  };

  return (
    <>
      <motion.nav
        initial={{ opacity: 0, y: -20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.6 }}
        className="fixed top-0 left-0 right-0 bg-white/80 backdrop-blur-lg z-50 h-18 shadow-md"
      >
        <div className="container-max h-full flex lg:grid lg:grid-cols-3 justify-between items-center px-4 sm:px-6 lg:px-8">
          {/* Izquierda: logo */}
          <div
            className="cursor-pointer flex items-center justify-start"
            onClick={() => scrollToSection(0)}
          >
            <img
              src="/LogoV2.png"
              alt="JEGASolutions logo"
              className="h-12 object-contain drop-shadow-lg lg:h-14"
            />
          </div>

          {/* Centro: Navegación */}
          <div className="hidden lg:flex justify-center space-x-8">
            {navItems.map(item => (
              <button
                key={item.label}
                onClick={() => scrollToSection(item.sectionIndex)}
                className={`text-base text-gray-700 hover:text-jega-blue-900 transition-colors font-medium relative after:content-[''] after:absolute after:left-0 after:bottom-[-4px] after:w-full after:h-0.5 after:bg-jega-gold-500 after:transition-transform after:duration-300 after:ease-in-out ${
                  activeSection === item.sectionIndex
                    ? 'text-jega-gold-500 font-bold after:scale-x-100'
                    : 'after:scale-x-0 hover:after:scale-x-100'
                }`}
              >
                {item.label}
              </button>
            ))}
          </div>

          {/* Derecha: Botón CTA y Menú Móvil */}
          <div className="flex items-center justify-end">
            <div className="hidden lg:flex">
              <button
                onClick={() => scrollToSection(3)}
                className="btn-primary px-6 py-3 text-sm"
              >
                Ver Precios
              </button>
            </div>
            <div className="lg:hidden">
              <button
                onClick={() => setIsMenuOpen(!isMenuOpen)}
                className="p-2 rounded-md text-gray-700 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-jega-blue-500"
              >
                {isMenuOpen ? (
                  <X className="w-6 h-6" />
                ) : (
                  <Menu className="w-6 h-6" />
                )}
              </button>
            </div>
          </div>
        </div>
      </motion.nav>

      {/* Panel de Menú Móvil */}
      {isMenuOpen && (
        <motion.div
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          exit={{ opacity: 0, y: -20 }}
          className="fixed inset-0 top-18 z-40 bg-white/95 backdrop-blur-sm lg:hidden"
        >
          <div className="flex flex-col items-center justify-center h-full space-y-8">
            {navItems.map(item => (
              <button
                key={item.label}
                onClick={() => handleNavClick(item.sectionIndex)}
                className={`text-2xl font-semibold ${
                  activeSection === item.sectionIndex
                    ? 'text-jega-gold-500'
                    : 'text-gray-800'
                }`}
              >
                {item.label}
              </button>
            ))}
            <button
              onClick={() => handleNavClick(3)}
              className="btn-primary px-8 py-4 text-lg"
            >
              Ver Precios
            </button>
          </div>
        </motion.div>
      )}
    </>
  );
};

export default Header;
