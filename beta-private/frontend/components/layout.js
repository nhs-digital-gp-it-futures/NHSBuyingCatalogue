import React from 'react'

import Link from 'next/link'
import Head from 'next/head'

import NHSDigitalLogo from './nhs-digital-logo'
import UserAuthenticationNav from './user-authentication-nav'

import styles from '../styles/styles.scss'

export default ({children, title = 'Home', ...props}) => (
  <React.Fragment>
    <Head>
      <meta charSet="utf-8" />
      <meta name='viewport' content='initial-scale=1.0, width=device-width' />
      <title>{title} - NHS Digital Buying Catalogue</title>
    </Head>
    <header role="banner" aria-label="Site header">
      <nav aria-label="Main">
        <Link href="#content"><a className="sr" id="skip-to-main-content-link">Skip to main content</a></Link>
        <Link href="/#content">
          <a aria-label="Home"><NHSDigitalLogo /></a>
        </Link>
        <UserAuthenticationNav {...props} />
        <Link href="/about#content"><a>About</a></Link>
      </nav>
    </header>
    <main id="content">
      {children}
    </main>
    <footer role="banner" aria-label="Site footer">
      <section role="group" aria-labelledby="footer-about-section-label">
        <h2 id="footer-about-section-label" role="none">About</h2>
        <Link href="/about#catalogue"><a><span className="sr">About the</span> Catalogue</a></Link>
        <Link href="/about#nhs-digital"><a><span className="sr">About</span> NHS Digital</a></Link>
      </section>
    </footer>
  </React.Fragment>
)
