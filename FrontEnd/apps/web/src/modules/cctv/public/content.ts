/**
 * Public website copy — aligned with www.aarvii.in messaging (imported June 2026).
 */

export const PUBLIC_BRAND = {
  name: 'AARVII',
  legalName: 'Aarvii Technologies',
  tagline: 'Secure Your World with AARVII — professional CCTV security solutions, installation, and maintenance.',
  phone: '+91 98765 43210',
  email: 'info@aarvii.in',
  address: 'Pune, Maharashtra, India',
} as const;

export const PUBLIC_STATS = [
  { value: '500+', label: 'Installations' },
  { value: '24/7', label: 'Support' },
  { value: '5+', label: 'Years experience' },
  { value: '100%', label: 'Customer satisfaction' },
] as const;

export const PUBLIC_WHY_CHOOSE = [
  {
    title: 'Expert installation',
    description: 'Professional technicians ensure optimal camera placement and system configuration for AHD and IP systems.',
  },
  {
    title: 'Quick response',
    description: 'Fast emergency response and unlimited service calls for AMC plans.',
  },
  {
    title: 'Advanced technology',
    description: 'Latest AHD and IP camera technology for crystal-clear surveillance.',
  },
] as const;

export const PUBLIC_SERVICES = [
  {
    title: 'CCTV Installation',
    description: 'Professional installation of AHD and IP cameras with optimal positioning for maximum coverage.',
  },
  {
    title: 'AMC Services',
    description: 'Comprehensive annual maintenance contracts with flexible plans and 24/7 support.',
  },
  {
    title: '24/7 Support',
    description: 'Round-the-clock technical support and emergency response for registered customers.',
  },
  {
    title: 'System Upgrades',
    description: 'Camera additions, storage upgrades, and network hardening for growing sites.',
  },
] as const;

export const PUBLIC_AMC_PLANS = [
  {
    code: 'BASIC',
    name: 'Basic AMC',
    visits: '2 visits / year',
    highlights: ['Scheduled preventive visits', 'Remote support', 'Standard SLA'],
  },
  {
    code: 'STANDARD',
    name: 'Standard AMC',
    visits: '4 visits / year',
    highlights: ['Quarterly preventive visits', 'Priority support', 'Health report after each visit'],
  },
  {
    code: 'PREMIUM',
    name: 'Premium AMC',
    visits: '6 visits / year',
    highlights: ['Bi-monthly visits', 'Critical SLA', 'Dedicated account coordinator'],
  },
] as const;

export const PUBLIC_TESTIMONIALS = [
  {
    quote: 'Aarvii keeps our warehouse cameras online year-round. Their AMC visits are always on schedule.',
    author: 'Operations Manager',
    company: 'Logistics client, Pune',
  },
  {
    quote: 'Responsive team — from installation to ticket resolution, we have full visibility through the portal.',
    author: 'IT Head',
    company: 'Manufacturing client',
  },
  {
    quote: 'Professional engineers and clear service reports after every visit. Highly recommended.',
    author: 'Facility Admin',
    company: 'Retail chain',
  },
] as const;

export const PUBLIC_GALLERY = [
  { caption: 'Multi-camera NVR deployment', category: 'Installation' },
  { caption: 'Preventive maintenance visit', category: 'AMC' },
  { caption: 'Retail store coverage layout', category: 'Design' },
  { caption: 'Warehouse perimeter monitoring', category: 'Installation' },
  { caption: 'Control room setup', category: 'Installation' },
  { caption: 'Engineer on-site service', category: 'AMC' },
] as const;
