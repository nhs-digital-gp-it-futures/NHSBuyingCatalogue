import React from 'react'

import Link from 'next/link'
import Head from 'next/head'

import NHSDigitalLogo from './nhs-digital-logo'

export default ({children, title = 'Home'}) => (
  <React.Fragment>
    <Head>
      <meta charSet="utf-8" />
      <meta name='viewport' content='initial-scale=1.0, width=device-width' />
      <title>{title} - NHS Digital Buying Catalogue</title>
    </Head>
    <header role="banner" aria-label="Site header">
      <nav aria-label="Main">
        <Link href="#content"><a id="skip-to-main-content-link">Skip to main content</a></Link>
        <Link href="/#content">
          <a aria-label="Home"><NHSDigitalLogo /></a>
        </Link>
        <Link href="/about#content"><a>About</a></Link>
      </nav>
    </header>
    <main id="content">
      {children}
    </main>
    <footer role="banner" aria-label="Site footer">
      <section>
        <h2>About</h2>
        <Link href="/about#catalogue"><a>Catalogue</a></Link>
        <Link href="/about#nhs-digital"><a>NHS Digital</a></Link>
      </section>
    </footer>
  </React.Fragment>
)
