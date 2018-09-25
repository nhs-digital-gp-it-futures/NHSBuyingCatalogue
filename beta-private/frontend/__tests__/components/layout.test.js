import { shallow } from 'enzyme'
import React from 'react'

import Layout from '../../components/layout.js'

describe('Layout component', () => {
  it('emits <title> tag beginning with supplied title prop', () => {
    const app = shallow(<Layout title='Something!' />)

    expect(app.find('title').text()).toEqual(
      expect.stringMatching(/^Something!/)
    )
  })
})

/* eslint-env jest */
