import { motion } from "framer-motion";
import {
  ArrowUp,
  Mail,
  Phone,
  MapPin,
  Linkedin,
  X,
  Github,
} from "lucide-react";

const footerLinks = {
  productos: [
    { name: "Gestión de Horas Extra", href: "#" },
    { name: "Reportes con IA", href: "#" },
    { name: "Calculadora de Precios", href: "#" },
  ],
  servicios: [
    { name: "Consultoría Tecnológica", href: "#" },
    { name: "Implementación", href: "#" },
    { name: "Soporte Técnico", href: "#" },
  ],
  empresa: [
    { name: "Sobre Nosotros", href: "#" },
    { name: "Carreras", href: "#" },
    { name: "Blog", href: "#" },
  ],
};

const socialLinks = [
  { icon: Linkedin, href: "#", label: "LinkedIn" },
  { icon: X, href: "#", label: "X" },
  { icon: Github, href: "#", label: "GitHub" },
];

const Footer = ({ onScrollToTop }) => {
  const year = new Date().getFullYear();

  return (
    <footer className="bg-gradient-to-b from-gray-900 to-black text-gray-400 relative section-padding">
      <div className="container-max">
        <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8 lg:gap-0 mb-12">
          <div className="lg:col-span-1 lg:pr-10">
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6 }}
              viewport={{ once: true }}
              className="-mt-4 mb-2"
            >
              <img
                src="/logo6.png"
                alt="JEGASolutions logo"
                className="h-16 object-contain"
              />
            </motion.div>
            <motion.p
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: 0.1 }}
              viewport={{ once: true }}
              className="text-base text-gray-400 mb-6 max-w-md leading-relaxed"
            >
              Transformamos procesos administrativos a través de tecnología
              innovadora y consultoría especializada.
            </motion.p>
          </div>

          {Object.entries(footerLinks).map(([title, links], index) => (
            <motion.div
              key={title}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: 0.1 + index * 0.1 }}
              viewport={{ once: true }}
              className="lg:px-2"
            >
              <h4 className="text-base text-white font-bold mb-4 capitalize">
                {title}
              </h4>
              <ul className="space-y-2">
                {links.map((link) => (
                  <li key={link.name}>
                    <a
                      href={link.href}
                      className="text-base text-gray-400 hover:text-jega-gold-400 transition-colors duration-300 hover:translate-x-1 inline-block"
                    >
                      {link.name}
                    </a>
                  </li>
                ))}
              </ul>
            </motion.div>
          ))}
        </div>

        <div className="border-t border-gray-800 pt-8">
          <div className="flex flex-col md:flex-row justify-between items-center space-y-4 md:space-y-0">
            <motion.p
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: 0.7 }}
              viewport={{ once: true }}
              className="text-base text-gray-500 text-center md:text-left"
            >
              © {year} JEGASolutions. Todos los derechos reservados.{" "}
              <span className="text-jega-gold-400 ml-2">
                Soluciones que nacen del corazón.
              </span>
            </motion.p>
            <motion.div
              initial={{ opacity: 0, x: 20 }}
              whileInView={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.6, delay: 0.6 }}
              viewport={{ once: true }}
              className="flex items-center space-x-4"
            >
              {socialLinks.map((link) => (
                <a
                  key={link.label}
                  href={link.href}
                  className="w-8 h-8 bg-gray-800 hover:bg-jega-gold-400 rounded-lg flex items-center justify-center transition-all duration-300 group hover:scale-110"
                >
                  <link.icon className="w-4 h-4 text-gray-400 group-hover:text-gray-900 transition-colors" />
                </a>
              ))}
            </motion.div>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
