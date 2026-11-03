/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
    extend: {
      colors: {
        ink: {
          50: '#f4f7f8',
          100: '#e3eaed',
          200: '#c9d6dc',
          300: '#a3b8c2',
          400: '#7693a1',
          500: '#5a7786',
          600: '#4d6270',
          700: '#42525d',
          800: '#3a4650',
          900: '#343d45',
          950: '#22282e'
        },
        tide: {
          50: '#effaf8',
          100: '#d7f2ee',
          200: '#b3e5de',
          300: '#80d0c7',
          400: '#4fb4ab',
          500: '#359890',
          600: '#287a74',
          700: '#24625e',
          800: '#214f4c',
          900: '#1f4240'
        }
      },
      fontFamily: {
        display: ['"Fraunces"', 'Georgia', 'serif'],
        sans: ['"Source Sans 3"', 'Segoe UI', 'sans-serif']
      },
      boxShadow: {
        panel: '0 18px 50px -28px rgba(34, 40, 46, 0.45)'
      }
    }
  },
  plugins: []
}
