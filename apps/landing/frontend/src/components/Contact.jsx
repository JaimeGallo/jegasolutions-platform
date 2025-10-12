import { useState } from 'react';
import { motion } from 'framer-motion';
import emailjs from '@emailjs/browser';
import {
  Mail,
  Phone,
  MapPin,
  Linkedin,
  Instagram,
  Send,
  MessageCircle,
} from 'lucide-react';

// Icono SVG de WhatsApp
const WhatsAppIcon = ({ className }) => (
  <svg
    viewBox="0 0 24 24"
    fill="currentColor"
    className={className}
    xmlns="http://www.w3.org/2000/svg"
  >
    <path d="M17.472 14.382c-.297-.149-1.758-.867-2.03-.967-.273-.099-.471-.148-.67.15-.197.297-.767.966-.94 1.164-.173.199-.347.223-.644.075-.297-.15-1.255-.463-2.39-1.475-.883-.788-1.48-1.761-1.653-2.059-.173-.297-.018-.458.13-.606.134-.133.298-.347.446-.52.149-.174.198-.298.298-.497.099-.198.05-.371-.025-.52-.075-.149-.669-1.612-.916-2.207-.242-.579-.487-.5-.669-.51-.173-.008-.371-.01-.57-.01-.198 0-.52.074-.792.372-.272.297-1.04 1.016-1.04 2.479 0 1.462 1.065 2.875 1.213 3.074.149.198 2.096 3.2 5.077 4.487.709.306 1.262.489 1.694.625.712.227 1.36.195 1.871.118.571-.085 1.758-.719 2.006-1.413.248-.694.248-1.289.173-1.413-.074-.124-.272-.198-.57-.347m-5.421 7.403h-.004a9.87 9.87 0 01-5.031-1.378l-.361-.214-3.741.982.998-3.648-.235-.374a9.86 9.86 0 01-1.51-5.26c.001-5.45 4.436-9.884 9.888-9.884 2.64 0 5.122 1.03 6.988 2.898a9.825 9.825 0 012.893 6.994c-.003 5.45-4.437 9.884-9.885 9.884m8.413-18.297A11.815 11.815 0 0012.05 0C5.495 0 .16 5.335.157 11.892c0 2.096.547 4.142 1.588 5.945L.057 24l6.305-1.654a11.882 11.882 0 005.683 1.448h.005c6.554 0 11.89-5.335 11.893-11.893a11.821 11.821 0 00-3.48-8.413Z" />
  </svg>
);

const contactInfo = [
  {
    icon: Mail,
    title: 'Email',
    value: 'JaimeGallo@jegasolutions.co',
    description: 'Respuesta en 24 horas',
    link: 'mailto:JaimeGallo@jegasolutions.co',
  },
  {
    icon: WhatsAppIcon,
    title: 'WhatsApp',
    value: '+57 3136093516',
    description: 'Lun - Vie: 8:00 AM - 6:00 PM',
    link: 'https://wa.me/573136093516?text=Hola,%20me%20gustaría%20conocer%20más%20sobre%20los%20servicios%20de%20JEGASolutions',
  },
  {
    icon: MapPin,
    title: 'Ubicación',
    value: 'Medellín, Colombia',
    description: 'Oficina principal',
  },
];

const socialLinks = [
  { icon: Linkedin, href: '#', label: 'LinkedIn' },
  { icon: Instagram, href: '#', label: 'Instagram' },
];

const Contact = () => {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    message: '',
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleInputChange = e => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);

    // Verificación preventiva: variables de entorno
    const SERVICE_ID = import.meta.env.VITE_EMAILJS_SERVICE_ID;
    const TEMPLATE_ID = import.meta.env.VITE_EMAILJS_TEMPLATE_ID;
    const PUBLIC_KEY = import.meta.env.VITE_EMAILJS_PUBLIC_KEY;

    if (!SERVICE_ID || !TEMPLATE_ID || !PUBLIC_KEY) {
      console.error('❌ Variables de entorno EmailJS no configuradas correctamente');
      alert('Error interno: configuración de EmailJS faltante.');
      setIsSubmitting(false);
      return;
    }

    try {
      const result = await emailjs.send(
        SERVICE_ID,
        TEMPLATE_ID,
        {
          from_name: formData.name,
          from_email: formData.email,
          message: formData.message,
          to_email: 'JaimeGallo@jegasolutions.co',
        },
        PUBLIC_KEY
      );

      console.log('✅ Email enviado con éxito:', result);

      setFormData({ name: '', email: '', message: '' });
      alert('¡Gracias por tu mensaje! Te contactaremos pronto.');
    } catch (error) {
      console.error('❌ Error al enviar el email:', error);
      alert(
        'Hubo un error al enviar tu mensaje. Por favor, intenta nuevamente o contáctanos directamente por WhatsApp.'
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <section className="section-padding bg-gradient-to-br from-blue-900 to-gray-900 text-white">
      <div className="container-max">
        <div className="flex flex-col h-full">
          {/* Header */}
          <motion.div
            initial={{ opacity: 0, y: -20 }}
            whileInView={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6 }}
            viewport={{ once: true }}
            className="text-center mb-6"
          >
            <h2 className="text-4xl md:text-5xl font-bold mb-3">
              Hablemos de tu <span className="gradient-text">Proyecto</span>
            </h2>
            <p className="text-lg text-gray-300 max-w-3xl mx-auto">
              Estamos listos para acompañarte en la transformación digital de tu
              empresa.
            </p>
          </motion.div>

          {/* Main Content */}
          <div className="flex-1 grid lg:grid-cols-5 gap-8">
            {/* Left Column - Contact Info */}
            <motion.div
              initial={{ opacity: 0, x: -30 }}
              whileInView={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.8 }}
              viewport={{ once: true }}
              className="lg:col-span-2 space-y-6"
            >
              <div>
                <h3 className="text-2xl font-bold mb-6">
                  Información de Contacto
                </h3>
                <div className="space-y-4">
                  {contactInfo.map((item, index) => {
                    const ItemIcon = item.icon;
                    const content = (
                      <>
                        <div className="w-10 h-10 bg-jega-gold-400/20 rounded-lg flex items-center justify-center flex-shrink-0 group-hover:bg-jega-gold-400/30 transition-colors">
                          <ItemIcon className="w-5 h-5 text-jega-gold-400" />
                        </div>
                        <div>
                          <h4 className="font-semibold text-white mb-1 text-base">
                            {item.title}
                          </h4>
                          <p className="text-jega-gold-300 font-medium mb-1 text-base">
                            {item.value}
                          </p>
                          <p className="text-gray-400 text-xs">
                            {item.description}
                          </p>
                        </div>
                      </>
                    );

                    return (
                      <motion.div
                        key={item.title}
                        initial={{ opacity: 0, y: 20 }}
                        whileInView={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.6, delay: index * 0.1 }}
                        viewport={{ once: true }}
                      >
                        {item.link ? (
                          <a
                            href={item.link}
                            target="_blank"
                            rel="noopener noreferrer"
                            className="flex items-start space-x-3 group hover:scale-105 transition-transform"
                          >
                            {content}
                          </a>
                        ) : (
                          <div className="flex items-start space-x-3 group">
                            {content}
                          </div>
                        )}
                      </motion.div>
                    );
                  })}
                </div>
              </div>

              {/* Social Links */}
              <div>
                <h4 className="text-xl font-semibold mb-4">Síguenos</h4>
                <div className="flex space-x-3">
                  {socialLinks.map((link, index) => (
                    <motion.a
                      key={link.label}
                      href={link.href}
                      initial={{ opacity: 0, scale: 0.8 }}
                      whileInView={{ opacity: 1, scale: 1 }}
                      transition={{ duration: 0.6, delay: index * 0.1 }}
                      viewport={{ once: true }}
                      className="w-10 h-10 bg-white/10 hover:bg-jega-gold-400/20 rounded-lg flex items-center justify-center transition-all duration-300 group hover:scale-110"
                    >
                      <link.icon className="w-4 h-4 text-white group-hover:text-jega-gold-400 transition-colors" />
                    </motion.a>
                  ))}
                </div>
              </div>
            </motion.div>

            {/* Right Column - Contact Form */}
            <motion.div
              initial={{ opacity: 0, x: 30 }}
              whileInView={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.8 }}
              viewport={{ once: true }}
              className="lg:col-span-3 bg-white/5 backdrop-blur rounded-xl p-6 border border-white/20"
            >
              <div className="flex items-center space-x-3 mb-4">
                <div className="w-8 h-8 bg-jega-gold-400 rounded-lg flex items-center justify-center">
                  <MessageCircle className="w-4 h-4 text-gray-900" />
                </div>
                <h3 className="text-2xl font-bold">Envíanos tu Consulta</h3>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="grid sm:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-300 mb-1">
                      Nombre completo *
                    </label>
                    <input
                      type="text"
                      name="name"
                      value={formData.name}
                      onChange={handleInputChange}
                      required
                      placeholder="Tu nombre completo"
                      className="w-full bg-white/10 border border-white/20 rounded-lg px-3 py-2 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-jega-gold-400 focus:border-transparent transition-all duration-300 text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-300 mb-1">
                      Correo electrónico *
                    </label>
                    <input
                      type="email"
                      name="email"
                      value={formData.email}
                      onChange={handleInputChange}
                      required
                      placeholder="tu@email.com"
                      className="w-full bg-white/10 border border-white/20 rounded-lg px-3 py-2 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-jega-gold-400 focus:border-transparent transition-all duration-300 text-sm"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">
                    Cuéntanos sobre tu proyecto *
                  </label>
                  <textarea
                    name="message"
                    value={formData.message}
                    onChange={handleInputChange}
                    required
                    rows="4"
                    placeholder="Describe tus necesidades específicas..."
                    className="w-full bg-white/10 border border-white/20 rounded-lg px-3 py-2 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-jega-gold-400 focus:border-transparent transition-all duration-300 resize-none text-sm"
                  ></textarea>
                </div>

                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="w-full btn-primary py-3 px-6 disabled:bg-gray-600 disabled:transform-none disabled:shadow-none flex items-center justify-center space-x-2 text-sm"
                >
                  {isSubmitting ? (
                    <>
                      <div className="w-4 h-4 border-2 border-gray-900 border-t-transparent rounded-full animate-spin"></div>
                      <span>Enviando...</span>
                    </>
                  ) : (
                    <>
                      <Send className="w-4 h-4" />
                      <span>Enviar Mensaje</span>
                    </>
                  )}
                </button>
              </form>
            </motion.div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default Contact;
